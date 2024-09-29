// program_test.go

package main

import (
	"fmt"
	"testing"
)

// TestProgram is a test for the program function
func TestShift(t *testing.T) {
	ar := []Space{Empty, Round, Round, Rock, Empty, Round}
	ex := []Space{Round, Round, Empty, Rock, Round, Empty}
	shift(ar)
	for i, v := range ar {
		if v != ex[i] {
			t.Errorf("all should be equal, found %d %c, expected %c", i, v, ex[i])
		}
	}
	fmt.Printf("%v\n", ar)
}


func TestA(t *testing.T) {
	sampleInput := `O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....`
	sampleOutput := `OOOO.#.O..
OO..#....#
OO..O##..O
O..#.OO...
........#.
..#....#.#
..O..#.O.O
..O.......
#....###..
#....#....`
	p := parse(sampleInput)
	fmt.Printf("%s\n", p)

	nxt := p.tilt(N)
	fmt.Printf("%s\n", nxt)
	load := nxt.load()
	ex := int64(136)
	if nxt.String() != sampleOutput+"\n" {
		t.Errorf("output incorrect")
	}
	if load != ex {
		t.Errorf("Load incorrect, found %d != %d", load, ex)
	}
}
