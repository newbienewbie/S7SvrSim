# -*- coding: UTF-8 -*-
import time
import shell

taper_id= shell.accept_input_int("请输入贴胶机号")

DB_INDEX = 200 + taper_id 
DB_OFFSET = 116

DB_OFFSET_STEP_NUM = DB_OFFSET + 2
DB_OFFSET_X = DB_OFFSET_STEP_NUM + 4
DB_OFFSET_Y = DB_OFFSET_X + 4
DB_OFFSET_A = DB_OFFSET_Y + 4

#####################

stepnum = shell.accept_input_int("请输入步序")
DB.WriteByte(DB_INDEX, DB_OFFSET_STEP_NUM, stepnum)

x= shell.accept_input_float("请输入X")
DB.WriteReal(DB_INDEX, DB_OFFSET_X, x)

y= shell.accept_input_float("请输入y")
DB.WriteReal(DB_INDEX, DB_OFFSET_Y, y)

a= shell.accept_input_float("请输入a")
DB.WriteReal(DB_INDEX, DB_OFFSET_A, a)

#############################

DB.WriteBit(DB_INDEX, DB_OFFSET, 0, True)
time.sleep(1)

# #################### Clean
DB.WriteBit(DB_INDEX, DB_OFFSET, 0, False)
DB.WriteByte(DB_INDEX, DB_OFFSET_STEP_NUM, 0)
DB.WriteReal(DB_INDEX, DB_OFFSET_X, x)
DB.WriteReal(DB_INDEX, DB_OFFSET_Y, y)
DB.WriteReal(DB_INDEX, DB_OFFSET_A, a)