# -*- coding: UTF-8 -*-
import time
import shell

taper_id= shell.accept_input_int("请输入贴胶机号")

DB_INDEX = 200 + taper_id 
DB_OFFSET = 170

DB.WriteBit(DB_INDEX, DB_OFFSET, 0, True)
time.sleep(1)
DB.WriteBit(DB_INDEX, DB_OFFSET, 0, False)


