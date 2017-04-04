import numpy as np
import functools as ft


def random_population(generator, count):
    return map(lambda _: generator(), range(count))


def evaluate_population(population, evaluator):
    simple = list(map(lambda s: (s, evaluator(s)), population))
    minimum = min(simple, key=lambda pair: pair[1])[1]
    substracted = list(map(lambda s: (s[0], s[1] - minimum), simple))
    total = ft.reduce(lambda acc, s: acc+s[1], substracted, 0)
    divided = list(map(lambda s: (s[0], s[1]/float(total)), substracted))
    return divided


def roulette_selector(survivor_count):
    def partial_sums(population):
        current = 0
        for pair in population:
            current += pair[1]
            yield (pair[0], pair[1], current)

    def bin_search(sums, throw):
        t = len(sums),
        if t == 1:
            return sums[0]
        if sums[t/2] > throw:
            return bin_search(sums[0..t/2], throw)
        else:
            return bin_search(sums[t/2..t-1], throw)

    def inner(population):
        sums = partial_sums(population)
        throws = np.random.uniform(0.0, 1.0, survivor_count)
        return map(lambda t: bin_search(sums,t), throws)
    return inner


def select_parents(cur_population, selector):
    return selector(cur_population)
    return 0


def crossover(population, crosser):
    # random pairs from population
    # 2 children per pair using crosser
    return 0


def mutate(population, mutator):
    return 0


def replace(children, parents, replacer):
    return 0


def simple_genetic_algorithm(dumper, dump_freq, terminator,
                             generator, count, evaluator,
                             selector, crosser, mutator, replacer):
    population = random_population(generator, count)
    cur_population = evaluate_population(population, evaluator)
    while not terminator():
        cur_population = select_parents(cur_population, selector)
        cur_population = crossover(cur_population, crosser)
        cur_population = mutate(cur_population, mutator)
        population = replace(cur_population, population, replacer)
        cur_population = evaluate_population(population, evaluator)


if __name__ == '__main__':
    def simple_evaluator(smth):
        return np.random.uniform()
    ev = list(evaluate_population(range(20), simple_evaluator))
