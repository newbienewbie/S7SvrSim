# -*- coding: UTF-8 -*-
import time
import shell

taper_id= shell.accept_input_int("请输入贴胶机号")

DB_INDEX = 200 + taper_id 
DB_OFFSET = 160
DB_OFFSET_CHANNEL = DB_OFFSET + 2


#############################

DB.WriteBit(DB_INDEX, DB_OFFSET, 0, True)
DB.WriteShort(DB_INDEX, DB_OFFSET_CHANNEL, 1)
time.sleep(100)

# #################### Clean
DB.WriteBit(DB_INDEX, DB_OFFSET, 0, False)
DB.WriteShort(DB_INDEX, DB_OFFSET_CHANNEL, 0)