import matplotlib
import sys
matplotlib.use('Agg')
import numpy as np
import es
import matplotlib.pyplot as plt


def square_sum(val):
    return -np.dot(val, np.transpose(val))


def shifted_square_sum(shift):
    return lambda val: square_sum(val+shift)


def griewank(val):
    n = len(val)
    fr = 4000
    s = 0
    p = 1
    i = 1
    for v in val:
        if abs(v) > 600:
            return -1000000
        s += v * v
        p *= np.cos(v/np.sqrt(i))
        i += 1
    return (-1) * (s/fr-p+1)


def ackley(val):
    n = len(val)
    a = 20
    b = 0.2
    c = 2*np.pi
    s1 = 0
    s2 = 0
    for v in val:
        if v > 30 or v < -15:
            return -1000000
        s1 += v*v
        s2 += np.cos(c*v)
    return (-1)*(-a * np.exp(-b*np.sqrt(1/n*s1)) - np.exp(1/n*s2) + a + np.exp(1))


def show_weird_func(xlo, xhi, func):
    xx, yy = np.meshgrid(np.linspace(xlo, xhi, 101),
                         np.linspace(xlo, xhi, 111))

    zz = np.zeros(xx.shape)
    for i in range(xx.shape[0]):
        for j in range(xx.shape[1]):
            zz[i, j] = func(np.array([xx[i, j], yy[i, j]]))

    plt.pcolor(xx, yy, zz)
    plt.colorbar()
    plt.show()


def test_weird_func(lo, hi, function, folder, name, dumpfreq, minScore, minGenerations, maxTime,
                    mu, lambd, sum, learning_coefficient):
    def run():
        generator = es.uniform_generator(lo, hi)
        dumper = es.super_dumper(folder, name, 1)
        terminator = es.super_terminator(minScore, minGenerations, maxTime)
        (tau, tau_zero) = es.get_taus(learning_coefficient, len(lo))
        replacer = es.mi_plus_lambda_replacer if sum else es.mi_lambda_replacer
        print("starting {0}".format(name))
        return es.evolution_strategy(dumper, terminator, generator, function, replacer, mu, lambd, tau, tau_zero)
    return run




if __name__ == '__main__':
    tests = []

    lo = np.array([-300] * 150)
    hi = np.array([300] * 150)

    tests.append(test_weird_func(lo, hi, square_sum, 'square', 'square_150_1_v3',
                                 5, None, None, 60*60*2, 500, 5000, False, 100))

    tests.append(test_weird_func(lo, hi, square_sum, 'square', 'square_150_1_sum',
                                 5, None, None, 60*60*2, 500, 5000, True, 100))

    lo = np.array([-300] * 30)
    hi = np.array([300] * 30)
    tests.append(test_weird_func(lo, hi, griewank, 'griewank', 'griewank_30_1',
                                 5, None, None, 60*60*2, 500, 5000, False, 100))
    tests.append(test_weird_func(lo, hi, griewank, 'griewank', 'griewank_30_1_sum',
                                 5, None, None, 60*60*2, 500, 5000, True, 100))

    lo = np.array([-12] * 30)
    hi = np.array([28] * 30)

    tests.append(test_weird_func(lo, hi, ackley, 'ackley', 'ackley_30_1',
                                 5, None, None, 60*60*2, 500, 5000, False, 3))
    tests.append(test_weird_func(lo, hi, ackley, 'ackley', 'ackley_30_1_sum',
                                 5, None, None, 60*60*2, 500, 5000, True, 3))

    for test in tests:
        test()


