import datetime
import math
import matplotlib
matplotlib.use('Agg')
import sga
import numpy as np


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

    def dumper(gen, bests, meds, worsts, bestdude, scoreofbest):
        dude = list(bestdude)
        sga.graph_and_save(filename)(gen, bests, meds, worsts, dude, scoreofbest)

    def evaluator(dude):
        try:
            total = 0
            for i in range(dim):
                for j in range(dim):
                    total -= flows[i][j]*dists[dude[i]][dude[j]]
        except IndexError:
            print("dude is {0}, {1} btw".format(dude, type(dude)))
            raise IndexError
        return total

    return sga.random_permutation_generator(0, dim), dumper, evaluator, dim


if __name__ == '__main__':
    files = [
             'Nug14',
             'Nug12',
             'Tai50a',
             'Nug30']

    for file in files:
        smth = workers_qap(file)
        print(smth)

        sga.simple_genetic_algorithm(smth[1], 10,
                                     sga.super_terminator(None, None, datetime.datetime.now()
                                                          + datetime.timedelta(0, smth[3]*50)),
                                     smth[0], 8000, smth[2], sga.roulette_selector(7000),
                                     sga.pmx_for_perms, sga.permutation_mutator(0, smth[3], 0.9),
                                     sga.sum_replacer)
