import pandas as pd
import numpy as np
from scipy.optimize import curve_fit
import os
import time
import pymysql
import sys
from keras.models import load_model


# 二次曲线拟合
def func(x, a, b, c):
    return a * np.sqrt(x) * (b * np.square(x) + c)


datamin = [0.0, -99.04266, -50.017166, -103.41745, -247.72955, -48.542183, 106.33297, 25.015251, 7.5066667]
datamax = [2598.1013, 60.0, 101.10883, 36.59494, 571.53564, 24.1073, 121.857765, 32.19089, 34987.742]


def rollback_normalize(alist, low, high):
    delta = high - low
    if delta != 0:
        for i in range(0, len(alist)):
            alist[i] = alist[i] * delta + low
    return alist


def predict(name):
    datamin = [0.0, -99.04266, -50.017166, -103.41745, -247.72955, -48.542183, 106.33297, 25.015251, 7.5066667]
    datamax = [2598.1013, 60.0, 101.10883, 36.59494, 571.53564, 24.1073, 121.857765, 32.19089, 34987.742]
    model_GRU = load_model('D:\\tensorflow\mode240.h5')
    # 测试数据
    #path = "E:\\PythonProjects\\BalloonTP\\data\\test1010\\20191010上午-57461-宜昌-探空仪01010799-全部-质控后数据.txt"
    dir = "E:\\PythonProjects\\BalloonTP\\data\\test109"
    path = os.path.join(dir, name)
    time_kick = 60
    record_count = 0
    data2 = [[0, 0, 0, 0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0, 0, 0, 0]]
    # data3 = [[0, 0, 0, 0, 0, 0, 0, 0, 0]]
    while True:
        print('当前读到了 %d' % record_count)
        if not os.path.exists(path):
            print('目标文件不存在')
            time.sleep(time_kick)
            continue
        myfile = open(path)
        lines = len(myfile.readlines())
        myfile.close()
        newdata = np.zeros((60, 9))
        data1 = np.zeros((1, 9))
        # db = pymysql.connect(host="localhost", user="root", password="root", db="detector")
        if (lines > record_count + 60):
            data = pd.read_table(path, header=0, index_col=0)
            values = data.values
            if (record_count + 60 > 300):
                x = [i for i in range(300)]
                x = np.array(x)
                y1 = np.array(values[record_count + 60 - 300:record_count + 60, 3])
                y2 = np.array(values[record_count + 60 - 300:record_count + 60, 4])
                y3 = np.array(values[record_count + 60 - 300:record_count + 60, 5])
                popt1, pcov1 = curve_fit(func, x, y1, maxfev=5000)
                popt2, pcov2 = curve_fit(func, x, y2, maxfev=5000)
                popt3, pcov3 = curve_fit(func, x, y3, maxfev=5000)
                a1 = popt1[0]
                b1 = popt1[1]
                c1 = popt1[2]
                a2 = popt2[0]
                b2 = popt2[1]
                c2 = popt2[2]
                a3 = popt3[0]
                b3 = popt3[1]
                c3 = popt3[2]
                yvals1 = func(x, a1, b1, c1)  # 拟合y值
                yvals2 = func(x, a2, b2, c2)  # 拟合y值
                yvals3 = func(x, a3, b3, c3)  # 拟合y值
                for k in range(300):
                    values[record_count + 60 - 300 + k][3] = yvals1[k]
                    values[record_count + 60 - 300 + k][4] = yvals2[k]
                    values[record_count + 60 - 300 + k][5] = yvals3[k]
            newdata = values[record_count:record_count + 60]
            data1 = np.sum(newdata, axis=0) / 60
            print(np.array(data1))

            for i in range(len(data1)):
                data1[i] = (data1[i] - datamin[i]) / (datamax[i] - datamin[i])
            data2[1] = data1
            # data2 = list(data2)
            # data2.append(data1)
            values1 = np.array(data2)
            scaled1 = values1.astype('float32')
            # scaler = MinMaxScaler(feature_range=(0, 1))
            # scaled1 = scaler.transform(values1)

            test_X = scaled1[1:, :9]
            print("test_X")
            print(test_X)
            test_XX = test_X.reshape((test_X.shape[0], 1, test_X.shape[1]))
            predict = model_GRU.predict(test_XX)
            id = 0;
            for i in range(240):
                predict[:, id] = float(rollback_normalize(predict[:, id], datamin[-3], datamax[-3]))
                predict[:, id + 1] = rollback_normalize(predict[:, id + 1], datamin[-2], datamax[-2])
                predict[:, id + 2] = rollback_normalize(predict[:, id + 2], datamin[-1], datamax[-1])

                # 打开数据库连接
                db = pymysql.connect(host="localhost", user="root", password="root", db="detector")
                # 使用cursor()方法获取操作游标
                cursor = db.cursor()
                t = time.strftime('%Y-%m-%d %H:%M:%S', time.localtime(time.time()))

                newname = name.split('.',1)[0]
                data = [newname, t, i, float(predict[0][id]), float(predict[0][id + 1]), float(predict[0][id + 2])]
                # print(float(predict[0][id]), float(predict[0][id+1]), float(predict[0][id+2]))
                sql = "INSERT INTO `predictlocation` (`name`, `time`, `timeid`, `lon`, `lat`,`alevel`) VALUES (%s,%s,%s,%s,%s,%s)"
                try:
                    cursor.execute(sql, data)  # 执行SQL语句
                    db.commit()  # 提交到数据库执行
                except:
                    # 发生错误时回滚
                    print("错误")
                    db.rollback()
                # 关闭数据库连接
                db.close()
                id = id + 3
            print(predict[0])

            # return predict[0]


        else:
            # db.close()
            break
        # 关闭数据库连接

        record_count = record_count + 60
        time.sleep(time_kick)

if __name__=='__main__':
    predict(sys.argv[1])