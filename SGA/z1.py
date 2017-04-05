import datetime
import math
import matplotlib
matplotlib.use('Agg')
import tspparser
import sga

def distance(c1, c2):
    a = c1[0] - c2[0]
    b = c1[1] - c2[1]
    return math.sqrt(a*a + b*b)



def workers_tsp(filename):
    cities = tspparser.produce_final("tsp/{0}.tsp".format(filename))
    size = len(cities)
    workers_tsp.bestest = None

    def dumper(gen, bests, meds, worsts, bestdude, scoreofbest):
        dude = list(bestdude)
        sga.graph_and_save(filename)(gen,bests,meds,worsts,dude,scoreofbest)
        if workers_tsp.bestest != dude:
            workers_tsp.bestest = dude
            tspparser.plot_cities(list(map(lambda i: cities[i], dude)), True, "{0}/path{1}".format(filename, gen))

    def evaluator(dude):
        total = 0
        for i in range(len(cities)-1):
            total -= distance(cities[dude[i]], cities[dude[i+1]])
        return total

    return sga.random_permutation_generator(0, size), dumper, evaluator, size


if __name__ == '__main__':
    files = ['berlin52',
             'kroA100',
             'kroA150',
             'kroA200']
    

    for file in files:
        smth = workers_tsp(file)
        sga.simple_genetic_algorithm(smth[1], 100,
                                     sga.super_terminator(None, None, datetime.datetime.now()
                                                          + datetime.timedelta(0, 1800)),
                                     smth[0], 10000, smth[2], sga.roulette_selector(8000),
                                     sga.pmx_for_perms, sga.permutation_mutator(0, smth[3], 0.9),
                                     sga.sum_replacer)
