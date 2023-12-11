package main

import (
	"fmt"
	"strconv"
	"strings"

	"github.com/samber/lo"
)

func Parse(input string) [][]int {
	lines := strings.Split(input, "\n")
	return lo.Map(lines, func(line string, _ int) []int {
		words := strings.Split(line, " ")
		return lo.Map(words, func(word string, _ int) int {
			i, err := strconv.Atoi(word)
			if err != nil {
				panic("bad input")
			}
			return i
		})
	})
}

func NextValue(input []int) int {
	if All(input, func(value int, idx int) bool {
		return value == 0
	}) {
		return 0
	}
	pairs := Pairwise(input)
	differences := lo.Map(pairs, func(pair lo.Tuple2[int, int], idx int) int {
		return pair.B - pair.A
	})
	return input[len(input)-1] + NextValue(differences)
}

func main() {
	input := `0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45`
	values := Parse(input)
	nexts := lo.Map(values, IgnoreIndex(NextValue))
	fmt.Println(lo.Sum(nexts))
}

func All[V any](collection []V, predicate func(item V, index int) bool) bool {
	return len(lo.Filter(collection, predicate)) == len(collection)
}

func Any[V any](collection []V, predicate func(item V, index int) bool) bool {
	return len(lo.Filter(collection, predicate)) >= 1
}

func Pairwise[V any](collection []V) []lo.Tuple2[V, V] {
	results := []lo.Tuple2[V, V]{}
	for i := 0; i < len(collection)-1; i++ {
		results = append(results, lo.T2(collection[i], collection[i+1]))
	}
	return results
}


func IgnoreIndex[V, K any](f func(V) K) func(V, int) K {
	return func(val V, idx int) K {
		return f(val)
	}
}