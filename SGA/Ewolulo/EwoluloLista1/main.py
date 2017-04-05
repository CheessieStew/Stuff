import numpy as np
import numpy.random as npr
import matplotlib.pyplot as plt
import os

def oneMax(bits):
    return sum(bits)


def deceptiveOneMax(bits):
    s = sum(bits)
    if s == 0:
        return len(bits) + 1
    else:
        return sum(bits)


def chunks(l, n):
    for i in range(0, len(l), n):
        yield l[i:i + n]


def kDeceptiveOneMax(k):
    return lambda bits: sum(map(deceptiveOneMax, chunks(bits, k)))


def pbil(function, terminator, dump, dumpsize, length, size, learn, mutprob, mutscale):
    def binaryRandom(p):
        if npr.uniform() < p:
            return 1
        else:
            return 0

    def randomIndividual(p):
        return np.vectorize(binaryRandom)(p)

    def randomPopulation(p, size):
        for i in range(0, size):
            yield (randomIndividual(p))

    prob = np.tile(0.5, length)
    pop = randomPopulation(prob, size)
    pop = list(map(lambda individual: (individual, function(individual)), pop))
    best = max(pop, key=lambda pair: pair[1])
    generation = 0
    bests = list([best[1]])
    probs = list([prob])
    overallBest = best
    while not (terminator(best, generation)):
        for k in range(0, length):
            prob[k] = prob[k] * (1 - learn) + best[0][k] * learn
        for k in range(0, length):
            if npr.uniform() < mutprob:
                prob[k] = prob[k] * (1 - mutscale) + binaryRandom(0.5) * mutscale
        pop = randomPopulation(prob, size)
        pop = list(map(lambda individual: (individual, function(individual)), pop))
        best = max(pop, key=lambda pair: pair[1])
        if overallBest[1] < best[1]:
            overallBest = best
        generation += 1
        bests.append(best[1])
        probs.append(np.copy(prob))
        print("proceed to generation {gen}".format(gen=generation))
        if (dumpsize != 0) and (generation % dumpsize == 0):
            dump(overallBest, bests, probs)
    return (overallBest, bests, probs)


def arrayFromFile(name, shape):
    #def fillArray(nums, array, dimensions):
    #    if len(dimensions) == 1:
    #        for i in range(dimensions[0]):
    #            array[i] = nums[0]
    #            nums.pop(0)
    #    else:
    #        for i in range(dimensions[0]):
    #            fillArray(nums, array[i], dimensions[1:])
    #            print("read{a}".format(a=i))
    #
    #res = np.empty(shape)
    with open(name) as f:
        lines = f.readlines()
        words = map(lambda l: l.split(), lines)
        floats = (map(lambda l: map(float, l), words))
        floats = [item for sublist in floats for item in sublist]
        return np.array(list(chunks(floats,shape[1])))


def graphAndSave(folderName):
    if not os.path.exists(folderName):
        os.makedirs(folderName)
    def graphDump(best, bests, probs):
        plt.figure(1)
        plt.subplot(210)
        plt.text(1, 1, "best score: {score}".format(score=best[1]))
        plt.subplot(211)
        plt.plot(range(0, len(bests)), bests)
        plt.subplot(212)
        plt.plot(range(0, len(probs)), probs)
        plt.savefig("{folder}/gen{gen}".format(folder=folderName, gen=len(bests)))
    return graphDump

def benchmark():
    pbil(oneMax, lambda b, g: g >= 100, graphAndSave("b1"), 10, 100, 200, 0.1, 0.05, 0.2)


def rateVector(target, rules):
    def aux(onoffs):
        corrects = 0
        for p in range(len(target)):
            votes = [0, 0, 0]
            for i in range(len(onoffs)):
                if onoffs[i] == 1:
                    votes[int(rules[i][p]) - 1] += 1
            winner = 0
            winvotes = 0
            for i in range(len(votes)):
                if votes[i] > winvotes:
                    winner = i+1
                    winvotes = votes[i]
            if target[p] == winner:
                corrects += 1
        return corrects / float(len(target))
    return aux


def images():
    sensei = arrayFromFile("ImageExpertReduced.txt", (9350, 1))
    print("sensei loaded")
    # problem = arrayFromFile("ImageRawReduced.txt", (9350, 3))
    # we don't even need the original values, since the rules are calculated already
    classificators = arrayFromFile("ClassificationRules.txt", (266, 9350))
    print("rules loaded")
    pbil(rateVector(sensei, classificators), lambda b, g: g >= 500, graphAndSave("2try3"), 1, 266, 100, 0.3, 0.05, 0.1)

if __name__ == '__main__':
    images()
