import numpy as np
import sys
import functools as ft
import matplotlib.pyplot as plt
import os
import datetime


class Specimen:
    value_chromosome = 12345
    mutation_chromosome = 12345
    note = 12345

    def __init__(self, value, mutation, note):
        self.value_chromosome = value
        self.mutation_chromosome = mutation
        self.note = note


def mi_plus_lambda_replacer(parents, children, mi):
    return sorted(children + parents, key=lambda specimen: specimen.note, reverse=True)[0:mi]


def mi_lambda_replacer(_, children, mi):
    return sorted(children, key=lambda specimen: specimen.note, reverse=True)[0:mi]


def new_mutation_chromosome(chromosome, tau, tau_zero):
    epsilons = np.random.normal(0, tau, len(chromosome))
    epsilon_zero = np.random.normal(0, tau_zero)
    return np.multiply(chromosome, np.exp(epsilons + epsilon_zero))

def new_value_chromosome(chromosome, mutation_chromosome):
    epsilons = np.array(list(
    map(lambda sigma: np.random.normal(0, sigma) if sigma > 0 else 0,
        mutation_chromosome)))
    return np.add(chromosome, epsilons)


def get_taus(learn_coefficient, specimen_len):
    return (learn_coefficient / np.sqrt(2*specimen_len),
            learn_coefficient / np.sqrt(2*np.sqrt(specimen_len)))


def pick_parents(candidates, count):
    partial_sums = []
    current = 0
    for specimen in sorted(candidates, key=lambda specimen: specimen.note):
        current += specimen.note
        partial_sums.append((specimen, current))

    def bin_search(sums, lo, hi, throw):
        t = hi - lo + 1
        if t == 1:
            return sums[lo][0]
        else:
            if t == 2:
                if (sums[lo])[1] < throw:
                    return sums[hi][0]
                else:
                    return sums[lo][0]
        if (sums[lo + t // 2])[1] > throw:
            return bin_search(sums, lo, lo + t // 2, throw)
        else:
            return bin_search(sums, lo + t // 2, hi, throw)

    throws = np.random.uniform(0, current, count)
    return list(map(lambda t: bin_search(partial_sums, 0, len(partial_sums) - 1, t), throws))


def evolution_strategy(dumper, terminator, generator, evaluator, replacer, mi, lambd, tau, tau_zero):
    def random_population():
        for i in range(0, mi):
            yield generator()

    def make_specimen(value):
        return Specimen(
            value,
            list(map(lambda v: 1 + np.sqrt(abs(v)), value)),
            evaluator(value)
        )

    def mutate(specimen):
        mutation = new_mutation_chromosome(specimen.mutation_chromosome, tau, tau_zero)
        value = new_value_chromosome(specimen.value_chromosome, mutation)
        res = Specimen(value, mutation, evaluator(value))
        return res

    population = list(random_population())
    population = list(map(make_specimen, population))

    generation = 0
    while not terminator(generation, population):
        dumper(generation, population)
        children = pick_parents(population, lambd)
        children = list(map(mutate, children))
        population = replacer(population, children, mi)
        generation += 1


def super_dumper(folder, name, freq):
    if not os.path.exists(folder):
        os.makedirs(folder)
    bests = []
    averages = []
    worsts = []
    stds = []
    best_so_far = [None]

    def the_dumper(gen, population):
        best = max(population, key=lambda specimen: specimen.note)
        med = ft.reduce(lambda acc, specimen: acc + specimen.note, population, 0) / len(population)
        worst = min(population, key=lambda specimen: specimen.note)
        std = np.std(list(map(lambda specimen: specimen.note, population)))
        bests.append(best.note)
        averages.append(med)
        worsts.append(worst.note)
        stds.append(std)

        if best_so_far[0] is None or best_so_far[0].note < best.note:
            best_so_far[0] = best
        if gen % freq == 0:
            fig = plt.figure(figsize=(10, 10))
            plt.subplot(211)
            plt.title("best score: {score}\n".format(
                score=best_so_far[0].note, specimen = best_so_far[0].value_chromosome))
            plt.plot(range(0, len(bests)), bests)
            plt.plot(range(0, len(bests)), averages)
            plt.plot(range(0, len(bests)), worsts)
            plt.subplot(212)
            plt.title("Standard deviations:")
            plt.plot(range(0, len(bests)), stds)
            plt.savefig('{0}/{1}'.format(folder,name))
            plt.close(fig)
        sys.stdout.flush()
    return the_dumper


def super_terminator(min_score, max_gens, max_time):
    deadline = datetime.datetime.now() + datetime.timedelta(0,max_time) if max_time is not None else None

    def the_terminator(gen, population):
        score = max(population, key=lambda specimen: specimen.note).note
        s = score >= min_score if score is not None and min_score is not None else False
        g = gen >= max_gens if max_gens is not None else False
        t = datetime.datetime.now() >= deadline if deadline is not None else False

        return s or g or t
    return the_terminator


def uniform_generator(low,high):
    def gen():
        return np.random.uniform(low,high)
    return gen
















