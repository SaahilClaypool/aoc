use std::collections::HashSet;
pub fn do_part_2(reg0: usize) -> (usize, i64) {
    let mut r0: i64 = reg0 as i64;
    let mut r1: i64 = 0;
    let mut r2: i64 = 0;
    let mut r3: i64 = 0;
    let mut r4: i64 = 0;
    let mut r5: i64 = 0;
    r5 = 123;
    r5 = r5 & 456;
    if r5 != 72 {
        return (0, 0);
    } else {
        r5 = 0;
    }
    r4 = r5 | 65536;
    r5 = 8858047;
    // loop
    let mut seen = HashSet::new();
    let mut last = r5;
    let mut breaker = 0;
    let mut counter = 0;
    'num8: loop {
        breaker += 1;
        if breaker > 100000 {
            return (0, 0);
        }
        r2 = r4 & 255;
        r5 = r5 + r2;
        r5 = r5 & 16777215;
        r5 = r5 * 65899;
        r5 = r5 & 16777215;
        if seen.contains(&r5) && last != r5 {
            panic!("result: {:?}: last {}", (breaker, r5), last);
        } else if !seen.contains(&r5) {
            println!("saw r5 = {}", r5);
            seen.insert(r5);
        }
        last = r5;
        if 256 > r4 {
            if r0 == r5 {
                // break 'num8;
            } else {
                continue 'num8;
            }
        }
        r2 = 0;
        'num18: loop {
            counter += 1;
            r1 = r2 + 1;
            r1 = r1 * 256;
            if r1 > r4 {
                r4 = r2;
                break 'num18;
            }
            r2 += 1;
        }
    }
    println!("halted with: {} count {}", reg0, counter); // basically we win here
    return (breaker, r0);
}
