import numpy as np
import functools as ft
import itertools as it
from random import shuffle
import matplotlib.pyplot as plt
import os
import datetime
import parser

def random_population(generator, count):
    return map(lambda _: generator(), range(count))


# (specimen OR specimen, target) -> (specimen,target)
def evaluate_population(population, evaluator):
    def eval_caller(s):
        if isinstance(s, tuple):
            return s if len(s) == 2 else (s[0], evaluator(s[0]))
        if isinstance(s, list):
            return s, evaluator(s)
        raise Exception("what the hell man, got a {0}".format(type(s)))
    return list(map(eval_caller, population))


# (specimen, target) -> (specimen, fitness)
def calculate_fitness(population):
    minimum = min(population, key=lambda pair: pair[1])[1]
    substracted = list(map(lambda s: (s[0], s[1] - minimum), population))
    total = ft.reduce(lambda acc, s: acc+s[1], substracted, 0)
    if total == 0:
        substracted = list(map(lambda s: (s[0], s[1] + 1), substracted))
        total += len(substracted)
    divided = list(map(lambda s: (s[0], s[1]/float(total)), substracted))
    return divided


def roulette_selector(survivor_count):
    def partial_sums(population):
        current = 0
        for pair in sorted(population, key=lambda p:p[1]):
            current += pair[1]
            yield (pair[0], current)

    def parent(sums, index):
        if (index == len(sums)):
            raise Exception("WATTT WHY")
        end = len(sums) - 2
        i = index
        while i < end and sums[i][0] is None:
            i += 1

        if sums[i][0] is not None:
            res = sums[i][0]
            sums[i] = (None, sums[i][1])
            return res
        i = index
        while i > 1 and sums[i][0] is None:
            i -= 1
        if sums[i][0] is not None:
            res = sums[i][0]
            sums[i] = (None, sums[i][1])
            return res
        raise RuntimeError('damnit dude why')

    def bin_search(sums, lo, hi, throw):
        t = hi-lo + 1
        if t == 1:
            return parent(sums, lo)
        else:
            if t == 2:
                if (sums[lo])[1] < throw:
                    return parent(sums, hi)
                else:
                    return parent(sums, lo)
        if (sums[lo + t//2])[1] > throw:
            return bin_search(sums, lo, lo+t//2, throw)
        else:
            return bin_search(sums, lo+t//2, hi, throw)

    def inner(population):
        sums = list(partial_sums(population))
        throws = np.random.uniform(0.0, 1.0, survivor_count)
        return list(map(lambda t: bin_search(sums, 0, len(sums)-1, t), throws))
    return inner


# (specimen, fitness) -> (specimen)
def select_parents(cur_population, selector):
    res = selector(cur_population)
    return res


# (specimen) -> (specimen)
def crossover(population, crosser):
    shuffle(population)
    t = len(population)
    males = population[0:t//2]
    females = population[t//2:t]
    for i in range(t//2):
        (c1, c2) = crosser(males[i], females[i])
        yield c1
        yield c2


# (specimen) -> specimen)
def mutate(population, mutator):
    for member in population:
        mut = mutator(member)
        if member != mut:
            yield member
            yield mut
        else:
            yield member


# (specimen, target)x2 -> (specimen, target)
def replace(children, parents, replacer, count, generator, evaluator):
    res = replacer(children, parents, count)
    res += evaluate_population(list(random_population(generator, count-len(res))), evaluator)
    return res


def sum_replacer(children, parents, count):

    total = list(children) + list(parents)
    a = list(sorted(calculate_fitness(total), key=lambda p: p[1]))
    freepass = -(int)(count * 0.80)

    new = list(roulette_selector(count)(a[0:freepass])) + list(map(lambda p: p[0], a[freepass:]))

    return new


def simple_genetic_algorithm(dumper, terminator,
                             generator, count, evaluator,
                             selector, crosser, mutator, replacer,
                             local_searcher = None):
    population = random_population(generator, count)
    population = evaluate_population(population, evaluator)
    cur_population = calculate_fitness(population)
    generation = 0
    bests = []
    meds = []
    worsts = []
    super_best_absolute = None
    super_best = None
    while not terminator(generation, super_best_absolute):
        generation += 1
        dumper(generation, population)

        cur_population = list(select_parents(cur_population, selector))
        cur_population = list(crossover(cur_population, crosser))

        cur_population = mutate(cur_population, mutator)
        cur_population = evaluate_population(cur_population, evaluator)
        if local_searcher is not None:
            cur_population += evaluate_population(local_searcher(population), evaluator)

        population = replace(cur_population, population, replacer, count, generator, evaluator)
        population = evaluate_population(population, evaluator)
        cur_population = calculate_fitness(population)

def any_in(iterable, func):
    for smth in iterable:
        if func(smth):
            return True
    return False


def random_permutation_generator(low, high):
    simple = list(range(low, high))

    def generator():
        shuffle(simple)
        res = []
        for val in simple:
            res.append(val)
        return res
    return generator


def multiply_seq_by_trans(seq, trans, low):
    try:
        tmp = seq[trans[0]-low]
        seq[trans[0] - low] = seq[trans[1]-low]
        seq[trans[1] - low] = tmp
    except(IndexError):
        print(seq)
        print(len(seq))
        print(trans)
        raise IndexError

def permutation_mutator(low, high, chance):
    def mutator(perm):
        if np.random.uniform() > chance:
            return perm
        cp = list(perm)
        trans = np.random.random_integers(low, high-1, 2)
        while trans[1] == trans[0]:
            trans[1] = np.random.random_integers(low, high-1)
        multiply_seq_by_trans(cp, trans, low)
        return cp
    return mutator


def pmx_for_perms(p1, p2):
    cuts = np.random.random_integers(0, len(p1) - 1, 2)
    if cuts[1] < cuts[0]:
        tmp = cuts[0]
        cuts[0] = cuts[1]
        cuts[1] = tmp
    c1 = [None] * len(p1)
    c2 = [None] * len(p2)
    for i in range(cuts[0],cuts[1]+1):
        c1[i] = p1[i]
        c2[i] = p2[i]

    for i in range(cuts[0],cuts[1]+1):
        if p2[i] not in c1:
            j = p2.index(p1[i])
            while j >= cuts[0] and j <= cuts[1]:
                j = p2.index(p1[j])
            c1[j] = p2[i]
        if p1[i] not in c2:
            j = p1.index(p2[i])
            while j >= cuts[0] and j <= cuts[1]:
                j = p1.index(p2[j])
            c2[j] = p1[i]
    for i in range(0, len(p1)):
        if c1[i] is None:
            c1[i] = p2[i]
        if c2[i] is None:
            c2[i] = p1[i]
    if any_in(c1, lambda x: x is None) or any_in(c2, lambda x: x is None):
        print('problem')
        print(p1)
        print(p2)
        print(c1)
        print(c2)
        print(cuts[0],cuts[1])
    return c1, c2


def graph_and_save(folderName):
    if not os.path.exists(folderName):
        os.makedirs(folderName)

    def graph_dump(gen, bests, meds, worsts, stds, scoreofbest):
        fig = plt.figure(figsize=(10, 10))
        plt.subplot(211)
        plt.title("best score: {score}\n Bests, averages and worsts:".format(score=scoreofbest))
        plt.plot(range(0, len(bests)), bests)
        plt.plot(range(0, len(bests)), meds)
        plt.plot(range(0, len(bests)), worsts)
        plt.subplot(212)
        plt.title("Standard deviations:")
        plt.plot(range(0, len(bests)), stds)
        plt.savefig('{0}/graph'.format(folderName))
        plt.close(fig)
    return graph_dump


def super_terminator(minscore, maxgens, maxtime):
    def terminator(gen, score):
        s = score >= minscore if score is not None and minscore is not None else False
        g = gen >= maxgens if maxgens is not None else False
        t = datetime.datetime.now() >= maxtime if maxtime is not None else False

        return s or g or t
    return terminator



def test():
    def one_max(bits):
        return sum(bits)

    def simple_crosser(bits1, bits2):
        t = len(bits1)
        cut = np.random.random_integers(0, t)
        c1 = bits1[0:cut] + (bits2[cut:len(bits2)] if cut < t else [])
        c2 = bits2[0:cut] + (bits1[cut:len(bits1)] if cut < t else [])
        return c1, c2

    def binary_random(p):
        if np.random.uniform() < p:
            return 1
        else:
            return 0

    def random_negator(chance):
        def mutator(bits):
            if np.random.uniform() < chance:
                cp = list(bits)
                i = np.random.random_integers(0, len(bits)-1)
                cp[i] = 1 - cp[i]
                return cp
            return bits
        return mutator

    def generator():
        return list(map(binary_random, [0.5] * 20))


    simple_genetic_algorithm(graph_and_save("asdf"), 5,
                             super_terminator(None,None,datetime.datetime.now()
                                              + datetime.timedelta(0, 5)),
                             generator, 500, one_max, roulette_selector(400),
                             simple_crosser, random_negator(0.1), sum_replacer)
    print('lol')

if __name__ == '__main__':
    test()
