// use std::error::Error;
use std::error::Error;
use std::fs::File;
use std::io::prelude::*;
use std::io::BufReader;
use std::str;
use std::time::Instant;

type Res<T> = Result<T, Box<dyn Error>>;

fn main() {
    let start = Instant::now();
    let input = parse_input("input.txt").unwrap();
    let count = make_map(&input);
    let end = Instant::now();
    let dur = end.duration_since(start);
    let millis = dur.as_secs() * 1000 + dur.subsec_millis() as u64;
    println!("Finished in : {} ms", millis);
    println!("answer is {}", count);

    let start = Instant::now();
    let input = parse_input("input.txt").unwrap();
    let count = safe_reagion(&input, 10000);
    let end = Instant::now();
    let dur = end.duration_since(start);
    let millis = dur.as_secs() * 1000 + dur.subsec_millis() as u64;
    println!("Finished in : {} ms", millis);
    println!("answer is {}", count);
}

const REASONABLE_MAX: i32 = 10000;

type Point = (i32, i32);

trait IPoint: Sized {
    fn dist(&self, other: &Self) -> i32 {
        (self.x() - other.x()).abs() + (self.y() - other.y()).abs()
    }
    fn x(&self) -> i32;
    fn y(&self) -> i32;
    fn closest_to(&self, points: &[Self]) -> Self;
}

impl IPoint for Point {
    fn x(&self) -> i32 {
        self.0
    }
    fn y(&self) -> i32 {
        self.1
    }

    /// note: doesn't handle points of same closeness
    fn closest_to(&self, points: &[Self]) -> Self {
        points.iter().fold((1000, 1000), |current, other| {
            if self.dist(&current) > self.dist(other) {
                return *other;
            } else {
                return current;
            }
        })
    }
}

fn make_map(points: &[Point]) -> u32 {
    let ((xmin, xmax), (ymin, ymax)) = get_bounds(points);
    let ((xmin, xmax), (ymin, ymax)) = (
        (xmin - REASONABLE_MAX, xmax + REASONABLE_MAX),
        (ymin - REASONABLE_MAX, ymax + REASONABLE_MAX),
    );
    let mut close_map: Vec<Point> = Vec::new();
    for x in xmin..=xmax {
        for y in ymin..=ymax {
            let p = (x, y);
            let closest = p.closest_to(points);
            close_map.push(closest);
        }
    }
    let mut max_close = 0;
    let mut max_point = (1000, 1000);
    for p in points {
        let count_close = close_map.iter().filter(|closest| *closest == p).count() as i32;
        if count_close > max_close && count_close < REASONABLE_MAX {
            max_close = count_close;
            max_point = *p;
        }
    }
    return max_close as u32;
}

fn parse_input(filename: &str) -> Res<Vec<Point>> {
    let f = File::open(filename)?;
    let reader = BufReader::new(f);

    let mut points = Vec::new();

    for line in reader.lines() {
        let line = line?;
        let mut split = line.split(",");
        let x = split.nth(0).unwrap().parse::<i32>()?; // getting the 0th element consumes it. The new first is the 0
        let y = split.nth(0).unwrap().trim().parse::<i32>()?;
        points.push((x, y));
    }
    Ok(points)
}

fn get_bounds(points: &[Point]) -> (Point, Point) {
    let mut min_x = 1000;
    let mut max_x = 0;
    let mut min_y = 1000;
    let mut max_y = 0;
    for point in points {
        let (x, y) = *point;
        if x < min_x {
            min_x = x;
        }
        if x > max_x {
            max_x = x;
        }
        if y < min_y {
            min_y = y;
        }
        if y > max_y {
            max_y = y;
        }
    }
    return ((min_x, max_x), (min_y, max_y));
}

fn safe_reagion(points: &[Point], cutoff: i32) -> i32 {
    let extend = 100;
    // calculate the
    let ((xmin, xmax), (ymin, ymax)) = get_bounds(points);
    let mut reagion = 0;
    for x in xmin - extend..xmax + extend {
        for y in ymin - extend..ymax + extend {
            let total_distance: i32 = points.iter().map(|p| p.dist(&(x, y))).sum();
            if total_distance < cutoff {
                reagion += 1
            }
        }
    }
    return reagion;
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test() {
        assert_eq!(0, 0);
    }

    #[test]
    fn test_input() {
        let parsed = parse_input("test.txt").unwrap();
        let count = make_map(&parsed);
        assert_eq!(count, 17);
    }

    #[test]
    fn test_safe() {
        let parsed = parse_input("test.txt").unwrap();
        let count = safe_reagion(&parsed, 32);
        assert_eq!(count, 16);
    }
}
