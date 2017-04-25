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


def workers_qap(dumptarget, filename):

    (dim, dists, flows) = read_qap("qap/{0}.dat".format(filename))
    print("{0}, {0}, {0}, {0}, {0}, {0}".format(dim, dists.shape[0], dists.shape[1], flows.shape[0], flows.shape[1]))
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
            workers_qap.bestSoFar = best
        
        sga.graph_and_save("z2b/{0}".format(filename))(gen, workers_qap.bests, workers_qap.meds, workers_qap.worsts,
                                                       workers_qap.stds, workers_qap.bestSoFar[1])
        

    def evaluator(dude):
        try:
            total = 0
            for i in range(dim):
                for j in range(dim):
                    total -= flows[i][j]*dists[dude[i]][dude[j]]
        except IndexError as e:
            print("dude is {0}, {1} btw".format(dude, type(dude)))
            raise e
        return total
    return sga.random_permutation_generator(0, dim), dumper, evaluator, dim


def localsearcher(evaluator, dim, intensity):
    def searcher(population):
        bestsofar = max(population, key=lambda pair: pair[1])
        betters = []
        chosen = np.random.random_integers(0, len(population)-1, intensity)
        mutator = sga.permutation_mutator(0,dim,1)
        for i in chosen:
            new = mutator(population[i][0])
            t = evaluator(new)
            if t > bestsofar[1]:
                betters.append((new, t))
        return betters
    return searcher




if __name__ == '__main__':
    sys.stdout = open('z2b_try_X.txt', 'w')
    print("smth")

    files = [
            ('nug30',6*60*60),
            ('tai40a', 7*60*60)]

    for file in files:
        try:
            smth = workers_qap("localsadist", file[0])
            print("\n\nSTARTED: {0}\n\n".format(file))
            sga.simple_genetic_algorithm(smth[1],
                                         sga.super_terminator(None, None, datetime.datetime.now()
                                                              + datetime.timedelta(0,
                                                                                   file[1])),
                                         smth[0], 5000, smth[2], sga.roulette_selector(4500),
                                         sga.pmx_for_perms, sga.permutation_mutator(0, smth[3], 0.9),
                                         sga.sum_replacer,
                                         localsearcher(smth[2], smth[3], smth[3] * smth[3] * smth[3] * smth[3]))
            print("next")
        except Exception:
            print(Exception)
            sys.stdout.flush()

    sys.stdout.close()
