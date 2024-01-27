# -*- coding: UTF-8 -*-
import time
import shell


DB_INDEX = 200 
DB_OFFSET = 110
DB_OFFSET_PACKCODE = DB_OFFSET + 2

DB.WriteString(DB_INDEX, DB_OFFSET_PACKCODE, 40, "abcdefghijklmn")
DB.WriteBit(DB_INDEX, DB_OFFSET, 0, True)
time.sleep(1)
DB.WriteBit(DB_INDEX, DB_OFFSET, 0, False)
DB.WriteString(DB_INDEX, DB_OFFSET_PACKCODE, 40, "")


