import datetime
import math
import matplotlib
import sys
matplotlib.use('Agg')
import sga
import numpy as np
import functools as ft
import matplotlib.pyplot as plt


def distance(c1, c2):
    a = c1[0] - c2[0]
    b = c1[1] - c2[1]
    return math.sqrt(a * a + b * b)


def read_qap(path):
    with open(path) as f:
        line1 = f.readline()
        dim = int(line1)
        f.readline()
        distances = []
        flows = []
        for i in range(dim):
            distances.append(list(map(lambda s: float(s), f.readline().split())))
        f.readline()
        for i in range(dim):
            flows.append(list(map(lambda s: float(s), f.readline().split())))
        return dim, np.array(distances), np.array(flows)


def workers_qap(filename):

    (dim, dists, flows) = read_qap("qap/{0}.dat".format(filename))
    workers_qap.bests = []
    workers_qap.meds = []
    workers_qap.worsts = []
    workers_qap.bestSoFar = None
    workers_qap.stds = []
    def dumper(gen, population):
        best = max(population, key=lambda pair: pair[1])
        med = ft.reduce(lambda acc, pair: acc + pair[1], population, 0) / len(population)
        worst = min(population, key=lambda pair: pair[1])
        std = np.std(list(map(lambda p: p[1], population)))
        workers_qap.bests.append(best[1])
        workers_qap.meds.append(med)
        workers_qap.worsts.append(worst[1])
        workers_qap.stds.append(std)
        if workers_qap.bestSoFar is None \
                or workers_qap.bestSoFar[1] < best[1]:
            print("new best!")
            workers_qap.bestSoFar = best
        print("gen {0}".format(gen))
        if gen % 10 == 0:
            sga.graph_and_save("z2a/{0}".format(filename))(gen, workers_qap.bests, workers_qap.meds, workers_qap.worsts,
                                                       workers_qap.stds, workers_qap.bestSoFar[1])
            print("dumped")

    def evaluator(dude):
        total = 0
        for i in range(dim):
            for j in range(dim):
                try:
                    s = np.shape(flows)
                    meh = flows[i][j]
                    meh *= dists[dude[i]][dude[j]]
                    total -= meh
                except IndexError:
                    print("dude is {0}, {1} btw".format(dude, type(dude)))
                    a = dude[i]
                    b = dude[j]
                    raise IndexError
        return total

    return sga.random_permutation_generator(0, dim), dumper, evaluator, dim


if __name__ == '__main__':
    files = [
             'nug30']

    a = [0,1,2]
    b = list(a)
    a[1] = 7
    print(a)
    print(b)

    for file in files:
        smth = workers_qap(file)
        print("\n\nSTARTED: {0}\n\n".format(file))

        sga.simple_genetic_algorithm(smth[1],
                                     sga.super_terminator(None, None, datetime.datetime.now()
                                                          + datetime.timedelta(0,
                                                                               smth[3] * smth[3] * smth[3] * 0.018)),
                                     smth[0], 8000, smth[2], sga.roulette_selector(7000),
                                     sga.pmx_for_perms, sga.permutation_mutator(0, smth[3], 0.9),
                                     sga.sum_replacer)
