// program_test.go

package main

import (
	"testing"
)

func TestReor(t *testing.T) {
	r := [][]Space{
		{Empty, Empty, Rock},
		{Empty, Rock, Round},
		{Round, Empty, Rock},
	}
	tmp := DeepCopy(r)
	dst := DeepCopy(r)
	g1 := platform{grid: r}
	g2 := platform{grid: dst}

	reorientRows(N, r, tmp)
	g3 := platform{grid: tmp}
	if g1.String() == g3.String() {
		t.Errorf("N nothing changed \n%s\n%s", g1.String(), g3.String())
	}
	undoReorientRows(N, tmp, dst)

	s1 := g1.String()
	s2 := g2.String()
	if s1 != s2 {
		t.Errorf("N failed\n%s\n%s", s1, s2)
	}
	reorientRows(W, r, tmp)
	undoReorientRows(W, tmp, dst)
	s1 = g1.String()
	s2 = g2.String()
	if s1 != s2 {
		t.Errorf("W failed\n%s\n%s", s1, s2)
	}
	reorientRows(S, r, tmp)
	g3 = platform{grid: tmp}
	if g1.String() == g3.String() {
		t.Errorf("S nothing changed \n%s\n%s", g1.String(), g3.String())
	}
	undoReorientRows(S, tmp, dst)
	s1 = g1.String()
	s2 = g2.String()
	if s1 != s2 {
		t.Errorf("S failed\n%s\n%s", s1, s2)
	}
	reorientRows(E, r, tmp)
	g3 = platform{grid: tmp}
	if g1.String() == g3.String() {
		t.Errorf("E nothing changed \n%s\n%s", g1.String(), g3.String())
	}
	undoReorientRows(E, tmp, dst)
	s1 = g1.String()
	s2 = g2.String()
	if s1 != s2 {
		t.Errorf("E failed\n%s\n%s", s1, s2)
	}
}
func TestBSpin(t *testing.T) {
	g := parse(`O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....`)
	s, g := g.spin()
	ex := `.....#....
....#...O#
...OO##...
.OO#......
.....OOO#.
.O#...O#.#
....O#....
......OOOO
#...O###..
#..OO#....`
	if s != ex {
		t.Errorf("\n%s !=\n%s", s, ex)
	}
}
func TestB(t *testing.T) {
	b := solveBSample(`O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....`)
	ex := 64
	if b != ex {
		t.Errorf("found %d != %d", b, ex)
	}
}

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

	nxt := p.tilt(N)
	load := nxt.load()
	ex := 136
	if nxt.String() != sampleOutput+"\n" {
		t.Errorf("output incorrect")
	}
	if load != ex {
		t.Errorf("Load incorrect, found %d != %d", load, ex)
	}
}
