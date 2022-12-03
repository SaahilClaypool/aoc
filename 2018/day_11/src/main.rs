use std::fs::File;
use std::io::prelude::*;

fn main() {
    let serial = 9995;
    let data = get_input("input.txt");
    let best_point = calc_max_score(&data, serial, 3);

    println!("best point is: {:?}", best_point);

    let serial = 9995;
    let data = get_input("input.txt");
    let best_point_size = calc_best_size(&data, serial);

    println!("best size is: {:?}", best_point_size);
}

#[derive(Copy, Clone, Debug, PartialEq)]
struct Point {
    x: usize,
    y: usize
}

fn get_input(filename: &str) -> Vec<Vec<i32>> {
    // let mut f = File::open(filename).unwrap();
    // let mut content = String::new();
    // f.read_to_string(&mut content).unwrap();
    // let mut data = vec![];
    // for line in content.lines() {
    //     let mut row = vec![];
    //     for val in line.split_whitespace() {
    //         row.push(val.parse().unwrap())
    //     }
    //     data.push(row);
    // }
    let mut data = vec![];
    for _r in 0..300 {
        data.push((0..300).collect());
    }
    data
}

fn get(p: Point, data: &[Vec<i32>]) -> i32 {
    data[p.y - 1][p.x - 1]
}

fn score_rack(p: Point, data: &[Vec<i32>], serial: i32) -> i32 {
    let id = 10 + p.x;
    let power = id * p.y;
    let power = power as i32 + serial;
    let power = power * id as i32;
    let power = (power / 100) % 10;
    let power = power - 5;
    power
}

fn score_group(top_left: Point, data: &[Vec<i32>], serial: i32, size: usize) -> i32 {
    let mut score = 0;
    for row in 0..size {
        for col in 0..size {
            let p = Point {x: col + top_left.x , y: row + top_left.y};
            score += score_rack(p, data, serial);
        }
    }
    score
}

fn calc_max_score(data: &[Vec<i32>], serial: i32, size: usize) -> (Point, i32) {
    let mut max_point = Point {x: 0, y: 0};
    let mut max_score = 0;

    for r in 0..data.len() - size + 1 {
        for c in 0..data[0].len() - size + 1 {
            let p = Point {x: r, y: c};
            let score = score_group(p, data, serial, size);
            if score > max_score {
                max_score = score;
                max_point = p;
            }
        }
    }
    (max_point, max_score)
}

fn calc_best_size(data: &[Vec<i32>], serial: i32) -> (Point, usize) {
    let mut max_size = 1;
    let mut max_score = 0;
    let mut max_point = Point {x: 0, y: 0};
    for i in 1 .. 50{
        let (point, score) = calc_max_score(data, serial, i);
        if score > max_score {
            max_score = score;
            max_point = point;
            max_size = i;
        }
    }

    (max_point, max_size)
}


#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_basic() {
        let data = get_input("test.txt");
        let score = score_rack(Point { x: 3, y: 5 }, &data, 8);
        assert_eq!(score, 4);
    }

    #[test]
    fn test_all() {
        let data = get_input("test.txt");
        let best_point = calc_max_score(&data, 42, 3);
        assert_eq!(best_point.0, Point {x: 21, y: 61});
    }
}