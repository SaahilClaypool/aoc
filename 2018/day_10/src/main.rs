use lazy_static::lazy_static;
use regex::Regex;
use std::fs::File;
use std::io::prelude::*;
use std::io::BufReader;
use std::str::FromStr;
use std::string::String;

fn main() {
    let mut sim = Sim::load("input.txt");
    let mut cur_dist = sim.total_distance();
    let mut cur_dist_i = 0;
    let mut i = 5;
    let mut max_bottom = 0;
    let mut max_bottom_i = 0;

    let mut min_var = sim.variance();
    let mut min_var_i = 0;

    for i in 0..20000 {
        sim.step();
        let var = sim.variance();
        if var < min_var {
            min_var = var;
            min_var_i = i;
        }
    }
    println!("finished after: {} min_dist {} at {} bottoms {} at {}", sim.t, cur_dist, cur_dist_i, max_bottom, max_bottom_i);
    println!("min_var = {} i = {}", min_var, min_var_i);
    for l in &sim.past[min_var_i] {
        println!("light: {:?}", l);
    }

    let mut past: Vec<usize> = (0..sim.past.len()).collect();
    past.sort_by(|i, other| {
        sim.past_var[*i].cmp(&sim.past_var[*other])
    });


    for i in 0..5 {
        sim.write(past[i]);
        println!("\n\n");
    }
}

struct Sim {
    lights: Vec<Light>,
    past: Vec<Vec<Light>>,
    past_var: Vec<u32>,
    t: i32,
    bounds: (Point, Point)
}

impl Sim {
    fn load(filename: &str) -> Self {
        let file = File::open(filename).unwrap();
        let reader = BufReader::new(file);
        let mut lights: Vec<Light> = Vec::new();

        for line in reader.lines() {
            let line = line.unwrap();
            lights.push(line.parse().unwrap());
        }

        let sim = Sim {
            t: 0,
            lights: lights,
            bounds: ((0,0), (0,0)),
            past: Vec::new(),
            past_var: Vec::new(),
        };
        sim
    }
    
    fn variance(&self) -> f32 {
        let mut distances: Vec<f32> = vec![];
        for light in &self.lights {
            for other in &self.lights {
                let d = light.dist(other) as f32;
                distances.push(d);
            }
        }
        let mut avg_dist: f32 = distances.iter().cloned().sum();
        avg_dist /= distances.len() as f32;
        let sse: f32 = distances.iter().map(|d| {
            let v = d - avg_dist;
            v * v
        }).sum();
        sse / distances.len() as f32
    }

    fn total_distance(&self) -> u32 {
        self.lights.iter().map(|light| {
            let this_dist: u32 = self.lights.iter().map(|other| {
                light.dist(other)
            }).sum();
            this_dist
        }).sum()
    }

    fn step(&mut self) {
        self.past.push(self.lights.iter().cloned().collect());
        self.past_var.push(self.variance() as u32);
        for v in &mut self.lights {
            v.step();
        }
        self.bounds = self.get_bounds(self.t as usize);
        self.t += 1;
    }

    /// (top left) (bottom right)
    fn get_bounds(&self, iter: usize) -> (Point, Point) {
        let mut min_x = 0;
        let mut min_y = 0;
        let mut max_x = 0;
        let mut max_y = 0;
        for light in &self.past[iter] {
            if light.loc.0 < min_x {
                min_x = light.loc.0;
            }
            if light.loc.1 < min_y {
                min_y = light.loc.1;
            }
            if light.loc.0 > max_x {
                max_x = light.loc.0;
            }
            if light.loc.1 > max_y {
                max_y = light.loc.1;
            }
        }

        ((min_x, min_y), (max_x, max_y))
    }

    fn write(&self, iter: usize) {
        println!("writing {}", iter);
        let ((min_x, min_y), (max_x, max_y)) = self.get_bounds(iter);
        let points: Vec<Point> = self.past[iter].iter().map(|light| (light.loc.0, light.loc.1)).collect();
        eprintln!("bounds: {:?}", self.bounds);
        for y in 0..=max_y - min_y{
            for x in min_x..=max_x - min_x {
                if points.contains(&(x + min_x,y+min_y)) {
                    print!("#");
                } else {
                    print!(" ");
                }
            }
            print!("\n");
        }
    }

    fn on_bottom(&self) -> u32 {
        let ((min_x, min_y), (max_x, max_y)) = self.bounds;
        eprintln!("bot: {}", max_y);
        let points: Vec<Point> = self.lights.iter().map(|light| (light.loc.0, light.loc.1)).collect();
        let total = points.iter().filter(|p| p.1 == max_y).count();
        return total as u32;
    }
}

type Point = (i32, i32);

#[derive(PartialEq, Debug, Clone)]
struct Light {
    loc: Point,
    vel: Point,
}

impl Light {
    fn step(&mut self) {
        self.loc.0 += self.vel.0;
        self.loc.1 += self.vel.1;
    }
    fn dist(&self, other: &Self) -> u32 {
        ((self.loc.0 - other.loc.0).abs() + 
        (self.loc.1 - other.loc.1).abs()) as u32
    }
}

impl FromStr for Light {
    type Err = Box<dyn std::error::Error>;
    fn from_str(s: &str) -> Result<Self, Self::Err> {
        lazy_static! {
            static ref re: Regex = Regex::new(
                r"position=<\s*(?P<xp>-?\d*),\s*(?P<yp>-?\d*)> velocity=<\s*(?P<xv>-?\d*),\s*(?P<yv>-?\d*)>"
            )
            .expect("Bad regex");
        }
        let caps = re.captures(s).unwrap();
        let xp: i32 = caps.name("xp").unwrap().as_str().parse().unwrap();
        let yp: i32 = caps.name("yp").unwrap().as_str().parse().unwrap();
        let xv: i32 = caps.name("xv").unwrap().as_str().parse().unwrap();
        let yv: i32 = caps.name("yv").unwrap().as_str().parse().unwrap();

        Ok(Self {
            loc: (xp, yp),
            vel: (xv, yv),
        })
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test() {
        let st = "position=< 7,  0> velocity=<-1,  0>";
        let light = Light::from_str(st).unwrap();
        assert_eq!(
            light,
            Light {
                loc: (7, 0),
                vel: (-1, 0)
            }
        );

        let st = "position=<-2,  2> velocity=< 2,  0>";
        let light = Light::from_str(st).unwrap();
        assert_eq!(
            light,
            Light {
                loc: (-2, 2),
                vel: (2, 0)
            }
        );
    }

    #[test]
    fn bounds() {
        let sim = Sim::load("test.txt");
        let bounds = sim.get_bounds();
        assert_eq!(bounds, ((-6, -4), (15, 11)))
    }
    #[test]
    fn test_str() {
        let sim = Sim::load("test.txt");
        let sim_output = sim.to_str();
        let real_output = "\
........#.............
................#.....
.........#.#..#.......
......................
#..........#.#.......#
...............#......
....#.................
..#.#....#............
.......#..............
......#...............
...#...#.#...#........
....#..#..#.........#.
.......#..............
...........#..#.......
#...........#.........
...#.......#..........
";
eprintln!("{}", sim_output);
eprintln!("{}", real_output);
    assert_eq!(sim_output.len(), real_output.len())
    }

}
