# -*- coding: UTF-8 -*-

# define constants
INFO_DB_NUMBER = 6
INTEROP_DB_NUMBER = 22

# 1号线上料——发送交互请求
s7_server_svc.WriteBit(INTEROP_DB_NUMBER, 6, 1, False)
