# -*- coding: UTF-8 -*-
import time
import shell

taper_id= shell.accept_input_int("请输入贴胶机号")
DBIndex = 200 + taper_id 

flag = True
DB.WriteBit(DBIndex, 100, 0, flag)

count = 0
while True:
    count += 1
    time.sleep(0.5)
    flag = not flag
    DB.WriteBit(DBIndex, 100, 0, flag)

    if count > 10:
        break

