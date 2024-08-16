# kite

Kite is a statically typed, elegant, interpreted programming language.

## Declarations

Declare a variable with the `mut` or `let` keyword followed by the value.
```
mut x = 10;

let y = "Hello World!";
```

Variables declared with `let` are _constant_, whereas `mut` permits the variable value to be changed.

<br>

Variables can be assigned as `undef` to specify that a variable has not been fully initialised.
```
mut x = undef;
```

Note that variables declared with `let` cannot be initiated with `undef` as they are constant.

```
let x = undef;    // invalid
```

## Control Flow Statements

'If' statements provide a basis for control flow and are as follows.

```
if true {
    // condition is true
} else {
    // condition is false
}
```

<br>

'While' loop statements are executed while a given condition remains true.

```
mut count = 1;

while count <= 5 {
    echo count;
    count = count + 1;
}  // 1, 2, 3, 4, 5
```

