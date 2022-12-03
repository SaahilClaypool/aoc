use std::fs::File;
use std::io::prelude::*;
use std::str::FromStr;
use std::string;
use std::time::Instant;


fn main() {
    let start = Instant::now();
    solve_a();
    let end = Instant::now();

    println!("Finished A in : {} ms", end.duration_since(start).as_secs() * 1000 + end.duration_since(start).subsec_millis() as u64);

    let start = Instant::now();
    solve_b();
    let end = Instant::now();

    println!("Finished B in : {} ms", end.duration_since(start).as_secs() * 1000 + end.duration_since(start).subsec_millis() as u64);
}

fn solve_a() {
    let filename = "input.txt";
    let mut content = String::new();
    File::open(filename)
        .unwrap()
        .read_to_string(&mut content)
        .unwrap();
    let mut grid = Grid::from_str(&content).unwrap();
    for i in 0..100000 {
        match grid.step(false) {
            Err(res) => {
                println!("First crash: {}", res);
                break;
            }, 
            Ok(()) => {}
        }
    }
}

fn solve_b () {
    let filename = "input.txt";
    let mut content = String::new();
    File::open(filename)
        .unwrap()
        .read_to_string(&mut content)
        .unwrap();
    let mut grid = Grid::from_str(&content).unwrap();
    for i in 0..100000 {
        match grid.step(true) {
            Err(res) => {
                println!("crash: {}\nleft: {}", res, grid.num_carts);
            }, 
            Ok(()) => {}
        };
        if grid.num_carts == 1 {
            let last_loc = grid.find_cart();
            println!("Only one cart left! (x, y) = ({},{})", last_loc.1, last_loc.0);
            break;
        }
    }
}

/**
 * Idea: turn input text into a grid of paths.
 * rep the carts on a different array
 *
 * Then, just run the simulation
 */

/// grid struct to hold everything
struct Grid {
    bounds: (usize, usize),
    map: Vec<Vec<Loc>>,
    carts: Vec<Vec<Option<Cart>>>,
    gen: u32,
    num_carts: usize,
}

impl FromStr for Grid {
    type Err = Box<dyn std::error::Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut lines: Vec<Vec<Loc>> = vec![];
        let mut carts: Vec<Vec<Option<Cart>>> = vec![];
        let char_grid: Vec<Vec<char>> = s.lines().map(|line| line.chars().collect()).collect();
        let mut num_carts = 0;
        for (row, line) in s.lines().enumerate() {
            let mut line_vec: Vec<Loc> = vec![];
            let mut cart_vec = vec![];
            for (col, c) in line.chars().enumerate() {
                let space_type = match c {
                    '/' => SpaceType::CurveForward,
                    '\\' => SpaceType::CurveBackward,
                    '|' | '-' | '>' | '<' | '^' | 'v' => SpaceType::Straight,
                    '+' => SpaceType::Intersection,
                    ' ' => SpaceType::None,
                    _ => panic!("I can't handle that char {}", c),
                };
                let real_c = match c {
                    '>' | '<' => '-',
                    '^' | 'v' => '|',
                    _ => c,
                };
                line_vec.push(Loc { space_type, c: real_c});
                let cart_dir = match c {
                    '>' => Direction::Right,
                    '<' => Direction::Left,
                    '^' => Direction::Up,
                    'v' => Direction::Down,
                    _ => Direction::None,
                };
                match cart_dir {
                    Direction::None => {
                        cart_vec.push(None);
                    }
                    _ => {
                        cart_vec.push(Some(Cart {
                        dir: cart_dir,
                        loc: (row, col),
                        last_turn: 0,
                        has_moved: false,
                    }));
                    num_carts += 1;
                    }
                    ,
                };
            }
            lines.push(line_vec);
            carts.push(cart_vec);
        }
        Ok(Grid {
            bounds: (lines.len(), lines[0].len()),
            map: lines,
            carts: carts,
            gen: 0,
            num_carts: num_carts
        })
    }
}

impl Grid {
    fn find_cart(&self) -> (usize, usize) {
        for row in 0..self.bounds.0 {
            for col in 0..self.bounds.1 {
                if let Some(_) = self.carts[row][col] {
                    return (row, col);
                }
            }
        }
        return (0, 0);
    }
    fn step(&mut self, cont: bool) -> Result<(), String> {
        self.gen += 1;
        for row_idx in 0..self.bounds.0 {
            for col_idx in 0..self.bounds.1 {
                let cart = &mut self.carts[row_idx][col_idx];
                if let Some(_) = cart {
                    let res = self.move_cart(row_idx, col_idx);
                    match res {
                        Err(output) => {
                            self.num_carts -= 2;
                            if cont {
                                eprintln!("{}", output);
                            } else {
                                return Err(output);
                            }
                        }, 
                        Ok(()) => {}
                    }
                }
            }
        }
        for row_idx in 0..self.bounds.0 {
            for col_idx in 0..self.bounds.1 {
                let cart = &mut self.carts[row_idx][col_idx];
                if let Some(cart) = cart {
                    cart.has_moved = false;
                }
            }
        }
        Ok(())
    }
    fn move_cart(&mut self, row_idx: usize, col_idx: usize) -> Result<(), String> {
        let mut cart = None;
        let grid_cart = &mut self.carts[row_idx][col_idx];
        if let Some(ref grid_cart) = grid_cart {
            if grid_cart.has_moved {
                return Ok(());
            }
        }
        std::mem::swap(&mut cart, grid_cart);
        let mut cart = cart.unwrap();
        let on = &self.map[row_idx][col_idx];
        let new_dir = match on.space_type {
            SpaceType::Straight => cart.dir,
            SpaceType::CurveForward => {
                // check what is around the curve /
                match cart.dir {
                    Direction::Up => Direction::Right,
                    Direction::Left => Direction::Down,
                    Direction::Down => Direction::Left,
                    Direction::Right => Direction::Up,
                    Direction::None => Direction::None,
                }
            }
            SpaceType::CurveBackward => {
                // check what is around the curve \\
                match cart.dir {
                    Direction::Up => Direction::Left,
                    Direction::Left => Direction::Up,
                    Direction::Down => Direction::Right,
                    Direction::Right => Direction::Down,
                    Direction::None => Direction::None,
                }
            }
            SpaceType::Intersection => {
                let new_dir = match cart.last_turn {
                    0 => {
                        // last turn was right. Now we go left
                        match cart.dir {
                            Direction::Up => Direction::Left,
                            Direction::Left => Direction::Down,
                            Direction::Down => Direction::Right,
                            Direction::Right => Direction::Up,
                            Direction::None => Direction::None,
                        }
                    }
                    1 => {
                        // last turn was left, now we go straight
                        cart.dir
                    }
                    2 => {
                        // last turn was straight, now we got right
                        match cart.dir {
                            Direction::Up => Direction::Right,
                            Direction::Left => Direction::Up,
                            Direction::Down => Direction::Left,
                            Direction::Right => Direction::Down,
                            Direction::None => Direction::None,
                        }
                    }
                    _ => panic!("last turn is not in range: {}", cart.last_turn),
                };
                cart.last_turn += 1;
                cart.last_turn %= 3;
                new_dir
            }
            SpaceType::None => panic!("Cart shouldn't be on none..."),
        };

        let new_loc = match new_dir {
            Direction::Up => (row_idx - 1, col_idx),
            Direction::Down => (row_idx + 1, col_idx),
            Direction::Left => (row_idx, col_idx - 1),
            Direction::Right => (row_idx, col_idx + 1),
            Direction::None => (row_idx, col_idx),
        };
        // check the thing at the new location
        let at_new_loc = &mut self.carts[new_loc.0][new_loc.1];
        if let Some(ref other_cart) = at_new_loc {
            let mut new_none_chart = None;
            std::mem::swap(at_new_loc, &mut new_none_chart);
            return Err(format!("Crash! (x,y) = {},{} gen {}", new_loc.1, new_loc.0, self.gen));
        } else {
            let mut new_cart = Some(Cart {
                loc: new_loc,
                dir: new_dir,
                last_turn: cart.last_turn,
                has_moved: true,
            });
            std::mem::swap(&mut new_cart, &mut self.carts[new_loc.0][new_loc.1]);
            return Ok(());
        }
    }
}

impl ToString for Grid {
    fn to_string(&self) -> String {
        let mut s = String::new();
        for row in 0..self.bounds.0 {
            for col in 0..self.bounds.1 {
                if let Some(ref cart) = self.carts[row][col] {
                    let c = match cart.dir {
                        Direction::Up => '^',
                        Direction::Down => 'v',
                        Direction::Left => '<',
                        Direction::Right => '>',
                        Direction::None => 'X',
                    };
                    s.push(c);
                } else {
                    s.push(self.map[row][col].c);
                    // s.push(' ');
                }
            }
            s.push('\n');
        }
        s
    }
}

struct Cart {
    loc: (usize, usize),
    dir: Direction,
    /// 0 is right, 1 is left 2 is forward
    last_turn: usize,
    has_moved: bool,
}

struct Loc {
    space_type: SpaceType,
    c: char
}

enum Direction {
    Left,
    Up,
    Down,
    Right,
    None,
}

enum SpaceType {
    Straight,
    CurveForward,
    CurveBackward,
    Intersection,
    None,
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test() {
        let filename = "test1.txt";
        let mut content = String::new();
        File::open(filename)
            .unwrap()
            .read_to_string(&mut content)
            .unwrap();
        let grid = Grid::from_str(&content);
    }
    #[test]
    fn test_move() {
        let filename = "test1.txt";
        let mut content = String::new();
        File::open(filename)
            .unwrap()
            .read_to_string(&mut content)
            .unwrap();
        let mut grid = Grid::from_str(&content).unwrap();
        grid.step(false);
    }
}
