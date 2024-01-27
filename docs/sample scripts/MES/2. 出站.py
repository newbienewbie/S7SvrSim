# -*- coding: UTF-8 -*-
import time
import shell


DB_INDEX = 200 
DB_OFFSET = 156

DB.WriteBit(DB_INDEX, DB_OFFSET, 0, True)
time.sleep(1)
DB.WriteBit(DB_INDEX, DB_OFFSET, 0, False)


