# -*- coding: UTF-8 -*-
import time

# -*- coding: UTF-8 -*-
import time
import shell

taper_id= shell.accept_input_int("请输入贴胶机号")

DBIndex = 200 + taper_id
OFFSET = 101

DB.WriteBit(DBIndex, OFFSET, 0, True)
time.sleep(1)
DB.WriteBit(DBIndex, OFFSET, 0, False)


