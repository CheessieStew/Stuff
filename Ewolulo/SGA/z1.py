import datetime
import math
import os
import matplotlib
import functools as ft
import numpy as np
matplotlib.use('Agg')
import tspparser
import sga

def distance(c1, c2):
    a = c1[0] - c2[0]
    b = c1[1] - c2[1]
    return math.sqrt(a*a + b*b)



def workers_tsp(filename):
    cities = tspparser.produce_final("tsp/{0}.tsp".format(filename))
    dest = "z1/{0}".format(filename)
    if not os.path.exists(dest):
        os.makedirs(dest)
    size = len(cities)
    workers_tsp.bests = []
    workers_tsp.meds = []
    workers_tsp.worsts = []
    workers_tsp.bestSoFar = None
    workers_tsp.stds = []
    def dumper(gen, population):
        best = max(population, key=lambda pair: pair[1])
        med = ft.reduce(lambda acc, pair: acc + pair[1], population, 0) / len(population)
        worst = min(population, key=lambda pair: pair[1])
        std = np.std(list(map(lambda p: p[1], population)))
        workers_tsp.bests.append(best[1])
        workers_tsp.meds.append(med)
        workers_tsp.worsts.append(worst[1])
        workers_tsp.stds.append(std)
        if workers_tsp.bestSoFar is None \
                or workers_tsp.bestSoFar[1] < best[1]:
            tspparser.plot_cities(list(map(lambda i: cities[i], best[0])), True, "{0}/path{1}".format(dest, gen))
            print("new best!")
            workers_tsp.bestSoFar = best
        print("gen {0}".format(gen))
        if gen % 10 == 0:
            sga.graph_and_save(dest)(gen, workers_tsp.bests, workers_tsp.meds, workers_tsp.worsts,
                                                           workers_tsp.stds, workers_tsp.bestSoFar[1])
            print("dumped")



    def evaluator(dude):
        total = 0
        for i in range(len(cities)-1):
            total -= distance(cities[dude[i]], cities[dude[i+1]])
        return total

    return sga.random_permutation_generator(0, size), dumper, evaluator, size


if __name__ == '__main__':
    files = [
             'berlin',
             'kroA150',
             'kroA200']
    

    for file in files:
        smth = workers_tsp(file)
        sga.simple_genetic_algorithm(smth[1],
                                     sga.super_terminator(None, None, datetime.datetime.now()
                                                          + datetime.timedelta(0, 3000)),
                                     smth[0], 10000, smth[2], sga.roulette_selector(8000),
                                     sga.pmx_for_perms, sga.permutation_mutator(0, smth[3], 0.9),
                                     sga.sum_replacer)
