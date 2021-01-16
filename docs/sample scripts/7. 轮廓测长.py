# -*- coding: UTF-8 -*-

# define constants
INFO_DB_NUMBER = 6
INTEROP_DB_NUMBER = 22


# 读取上料工位的缓存
# 原料码
raw_barcode = s7_server_svc.ReadString(INFO_DB_NUMBER, 2780)
# 锯切段数
cutting_num = s7_server_svc.ReadByte(INFO_DB_NUMBER, 3048)
# 原料码去向
gotoline = s7_server_svc.ReadBit(INFO_DB_NUMBER,3049,0)
# 尺寸判断结果
size_checking = s7_server_svc.ReadBit(INFO_DB_NUMBER,3049,1)
# 理论尺寸
len_in_theory = s7_server_svc.ReadReal(INFO_DB_NUMBER,3050)
width_in_thoery = s7_server_svc.ReadReal(INFO_DB_NUMBER,3054)
depth_in_theory = s7_server_svc.ReadReal(INFO_DB_NUMBER,3058)
cutting_code = s7_server_svc.ReadString(INFO_DB_NUMBER,3062)

# 半成品码条码
banchengpin1_barcode = s7_server_svc.ReadString(INFO_DB_NUMBER,3318)
banchengpin2_barcode = s7_server_svc.ReadString(INFO_DB_NUMBER,3602)
banchengpin3_barcode = s7_server_svc.ReadString(INFO_DB_NUMBER,3886)
banchengpin4_barcode = s7_server_svc.ReadString(INFO_DB_NUMBER,4170)
banchengpin5_barcode = s7_server_svc.ReadString(INFO_DB_NUMBER,4454)




# 写入一号线的测长工位的原料码
s7_server_svc.WriteString(INFO_DB_NUMBER, 27800, raw_barcode)
# 写入一号线的测长工位的半成品码，这里假设是显示半成品码2
s7_server_svc.WriteString(INFO_DB_NUMBER, 28338, banchengpin2_barcode) 
# 发请求信号, 通知上位机去1200-1 PLC那请求测长数据
s7_server_svc.WriteBit(INTEROP_DB_NUMBER, 6, 2, True)
