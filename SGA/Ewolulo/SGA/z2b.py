import datetime
import math
import matplotlib
matplotlib.use('Agg')
import sga
import numpy as np
import heapq

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


def workers_qap(dumptarget, filename):
    (dim, dists, flows) = read_qap("qap/{0}.dat".format(filename))

    def dumper(gen, bests, meds, worsts, bestdude, scoreofbest):
        dude = list(bestdude)
        sga.graph_and_save("{0}/{1}".format(dumptarget, filename))(gen, bests, meds, worsts, dude, scoreofbest)

    def evaluator(dude):
        try:
            total = 0
            for i in range(dim):
                for j in range(dim):
                    total -= flows[i][j]*dists[dude[i]][dude[j]]
        except IndexError:
            print("dude is {0}, {1} btw".format(dude, type(dude)))
        return total

    return sga.random_permutation_generator(0, dim), dumper, evaluator, dim


def localsearcher(evaluator,samplesize,intensity):
    def searcher(population):
        bests = list(heapq.nlargest(samplesize, population, key=lambda pair: pair[1]))
        bestsofar = max(bests, key=lambda pair: pair[1])
        betters = []
        chosen = np.random.random_integers(0, samplesize - 1, intensity)
        for i in chosen:
            sga.multiply_seq_by_trans(bests[i][0], np.random.random_integers(0, len(bestsofar[0])-1, 2), 0)
            t = evaluator(bests[i][0])
            if t > bestsofar[1]:
                newbetter = []
                for v in bests[i][0]:
                    newbetter.append(v)
                betters.append((newbetter, t))
        if len(betters)>0:
            print("got {0} betters".format(len(betters)))
        return betters
    return searcher




if __name__ == '__main__':
    files = ['nug12',
             'nug14',
             'tai50a',
             'nug30']

    for file in files:
        smth = workers_qap("localsadist", file)
        print(smth)

        sga.simple_genetic_algorithm(smth[1], 10,
                                     sga.super_terminator(None, None, datetime.datetime.now()
                                                          + datetime.timedelta(0,
                                                                               smth[3] * smth[3] * smth[3] * 0.015)),
                                     smth[0], 1000, smth[2], sga.roulette_selector(900),
                                     sga.pmx_for_perms, sga.permutation_mutator(0, smth[3], 0.9),
                                     sga.sum_replacer, localsearcher(smth[2], 50, 4000))
        print('next')
