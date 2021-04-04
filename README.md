## Puyo_Controller

- `movey()`　
  - mpuyoとspuyoを-Configs.ymove分y方向にずらす.


- `movex(float hkey)`
  - mpuyoとspuyoをConfigs.xmove分x方向にずらす.

- `movex_isxmoving(float vkey)`　
  - aaa



## キモイポイント
 - is_xxxingのConfings.move_countがなんかきもい。

 - 同じくmovexとrotationの中でmposとsposの更新にハードコーディング。
    `this.mpos.x += (int)delta.x * 2;`

