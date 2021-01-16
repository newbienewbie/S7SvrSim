# -*- coding: UTF-8 -*-

# define constants
INFO_DB_NUMBER = 6
INTEROP_DB_NUMBER = 22


# 1号线上料——原料参数
s7_server_svc.WriteString(INFO_DB_NUMBER, 2780, "1908WC16V299F6+YSTC1100139+L2/L3:1757;L1/N:1762;")
s7_server_svc.WriteReal(INFO_DB_NUMBER, 3036, 3545.2)
s7_server_svc.WriteReal(INFO_DB_NUMBER, 3040, 68.3)
s7_server_svc.WriteReal(INFO_DB_NUMBER, 3044, 4.8)

# 1号线上料——发送请求
s7_server_svc.WriteBit(INTEROP_DB_NUMBER,6,0, True)