package main

import (
	"fmt"
	"os"
	"strings"
)

func main() {
	b := solveB()
	fmt.Printf("%d\n", b)
}

func solveA() int {
	b, err := os.ReadFile("input.txt")
	if err != nil {
		panic(err)
	}
	return solveASample(string(b))
}

func solveASample(input string) int {
	platform := parse(input)
	nxt := platform.tilt(N)
	return nxt.load()
}

func solveB() int {
	b, err := os.ReadFile("input.txt")
	if err != nil {
		panic(err)
	}
	return solveBSample(string(b))
}
func solveBSample(input string) int {

	platform := parse(input)
	s := make(map[string]any)
	for i := 0; i < 1000000000; i++ {
		var nl string
		nl, platform = platform.spin()
		if _, contains := s[nl]; contains {
			break
		}
		s[nl] = true
	}
	return platform.load()
}

func (p platform) spin() (string, platform) {
	p = p.tilt(N)
	nl := p.String()
	p = p.tilt(W)
	p = p.tilt(S)
	p = p.tilt(E)
	return nl, p
}

func parse(input string) platform {
	lines := strings.Split(input, "\n")
	w := len(lines[0])
	h := len(lines)
	grid := make([][]Space, h)
	for i, _ := range grid {
		grid[i] = make([]Space, w)
	}

	for row, line := range lines {
		for col, c := range line {
			grid[row][col] = Space(c)
		}
	}

	return platform{grid: grid}
}

func (p platform) tilt(d Dir) platform {
	n := DeepCopy(p.grid)
	final := DeepCopy(p.grid)
	reorientRows(d, p.grid, n)
	for _, row := range n {
		shift(row)
	}
	undoReorientRows(d, n, final)
	newPlatform := platform{grid: final}
	return newPlatform
}

func (p platform) load() int {
	h := int(len(p.grid))
	sum := int(0)
	for r, v := range p.grid {
		for _, c := range v {
			if c == Round {
				sum += h - int(r)
			}
		}
	}
	return sum
}

func undoReorientRows(d Dir, src, n [][]Space) {
	reorientRows(d, src, n)
}

func reorientRows(d Dir, src, n [][]Space) {
	switch d {
	case N:
		northRows(src, n)
	case E:
		eastRows(src, n)
	case S:
		southRows(src, n)
	case W:
		westRows(src, n)
	}
}

func westRows(src, dst [][]Space) {
	for r, row := range src {
		for c, v := range row {
			dst[r][c] = v
		}
	}
}

// cols top to bottom
func northRows(src, dst [][]Space) {
	for r, row := range src {
		for c, v := range row {
			dst[c][r] = v
		}
	}
}

func eastRows(src, dst [][]Space) {
	w := len(src[0])
	for r, row := range src {
		for c, v := range row {
			dst[r][w-c-1] = v
		}
	}
}

func southRows(src, dst [][]Space) {
	h := len(src)
	for r, row := range src {
		for c, v := range row {
			dst[h-r-1][c] = v
		}
	}
}

func shift(row []Space) {
	base := -1
	for i, v := range row {
		if v == Round {
			row[i] = Empty
			row[base+1] = Round
			base += 1
		} else if v == Empty {
			continue
		} else if v == Rock {
			base = i
		}
	}
}

type platform struct {
	grid [][]Space
}

func (p platform) String() string {
	s := ""
	for _, row := range p.grid {
		for _, c := range row {
			s += string(c)
		}
		s += "\n"
	}
	return s
}

type Pos struct {
	R int
	C int
}

type Space rune

const (
	Round Space = 'O'
	Empty Space = '.'
	Rock  Space = '#'
)

type Dir int

const (
	N Dir = iota
	E
	S
	W
)

func DeepCopy[T any](arr [][]T) [][]T {
	copiedArr := make([][]T, len(arr))
	for i := range arr {
		copiedArr[i] = make([]T, len(arr[i]))
		copy(copiedArr[i], arr[i])
	}
	return copiedArr
}
