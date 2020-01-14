import numpy as np
import math
import  pandas as pd
import  os
from keras.models import load_model
from sklearn.preprocessing import MinMaxScaler
import sys
earth_radius = 6370.856
pis_per_degree = math.pi / 180

name = []
num = [[] for i in range(3)]
List = [[] for i in range(3)]

stations={
    '宜昌': [111.3500, 30.7300],
    '武汉': [114.0500, 30.6000],
    '长沙': [112.7800, 28.0100],
    '赣州': [115.0000, 25.8700],
    '安庆': [116.9700, 30.6200],
    '南昌': [115.9011, 28.5900],
}

def trainBYS():

    files=os.listdir('E:\VSProjects\\test\\fangqiu\放球站数据'.encode('utf-8'))

    for file in files:
        path='E:\VSProjects\\test\\fangqiu\放球站数据\\'+file.decode('utf-8')
        name.append(file.decode('utf-8')[:2])

        for j in range(3):
            t=0
            temp={}
            with open(path, 'r') as f:
                read = f.readline()
                while len(read)>1:
                    x = read.split(' ')
                    length=int(x[2])

                    read = f.readline()
                    x1 = read.split(' ')
                    read = f.readline()
                    x2 = read.split(' ')
                    read = f.readline()
                    x3 = read.split(' ')

                    for i in range(length):
                        a=round(float(x1[i]),j)
                        b=round(float(x2[i]),j)
                        if tuple([a,b]) not in temp:
                            temp[tuple([a,b])] = 1
                        else:
                            temp[tuple([a, b])] += 1
                        t+=1
                    read = f.readline()
            List[j].append(t)
            num[j].append(temp)

def bys(x,y,z):

    trainBYS()

    if z<1:
        j=2
    elif z<10:
        j=1
    else:
        j=0

    L=[]
    time=0
    while j>=0:

        a = round(x, j)
        b = round(y, j)

        temp=[]
        sum=0
        for i in range(6):

            if tuple([a, b]) not in num[j][i]:
                t=0
            else:
                t=num[j][i][tuple([a, b])]
            t=t/List[j][i]
            sum+=t
            temp.append(t)

        #print("精确度：保留"+str(j)+"位小数")
        if sum==0:
            #print("历史未到达该区域，扩大范围进一步查找")
            print()
        else:
            #if time>1:
            #    print("历史上未达到过敏感区，但曾到达过敏感区附近")
            for i in range(6):
                for k in range(5)[::-1]:
                    if temp[k+1]>temp[k]:
                        one=temp[k+1]
                        temp[k+1]=temp[k]
                        temp[k]=one
                        two=name[k+1]
                        name[k+1]=name[k]
                        name[k]=two
                print(name[i]+":",end=' ')
                print(temp[i]/sum)
                temp[i] /= sum
            return name,temp

        j-=1
        time+=1

    return [],[]

def RNN(name):


    def create_dataset(data, n_predictions, n_next):
        '''
        对数据进行处理
        '''
        dim = data.shape[1]
        train_X, train_Y = [], []
        for i in range(data.shape[0] - n_predictions - n_next + 1):
            a = data[i:(i + n_predictions), :]
            train_X.append(a)
            train_Y.append(data[(i + n_predictions), :])
        train_X = np.array(train_X, dtype='float64')
        train_Y = np.array(train_Y, dtype='float64')

        return train_X, train_Y

    pre = 2
    dataframe = pd.read_csv('E:\VSProjects\\test\\fangqiu\minTrain\\'+name+'上午.csv', usecols=[1, 2], engine='python', skipfooter=0)
    dataset = dataframe.values
    # 将整型变为float
    dataset = dataset.astype('float32')

    scaler = MinMaxScaler(feature_range=(0, 1))
    dataset = scaler.fit_transform(dataset)

    testlist = dataset[-3:]

    test_X, test_Y = create_dataset(testlist, pre, 1)

    if os.path.exists('E:\VSProjects\\test\\fangqiu\RNN\\'+name+'.h5'):
        model = load_model('E:\VSProjects\\test\\fangqiu\RNN\\'+name+'.h5')
    else:
        #没有历史数据，无法预测，人为提供
        if name=='南昌':
            x1 = 116.09586720972494
            y1 = 28.524825115623162
            x2 = 116.0720298495216
            y2 = 28.52922204990136
            sin = (y2-y1)/((x1-x2)**2+(y1-y2)**2)**0.5
            cos = (x2-x1)/((x1-x2)**2+(y1-y2)**2)**0.5
            return sin, cos
        if name=='赣州':
            return 1,1

    testPredict = model.predict(test_X)

    testPredict = scaler.inverse_transform(testPredict)
    test_Y = scaler.inverse_transform(test_Y)

    sin=testPredict[0][0]
    cos=testPredict[0][1]
    return sin,cos

def lat_degree2km(dif_degree=.0001, radius=earth_radius):
    return radius * dif_degree * pis_per_degree

def lng_degree2km(dif_degree=.0001, center_lat=22):
    real_radius = earth_radius * math.cos(center_lat * pis_per_degree)
    return lat_degree2km(dif_degree, real_radius)

def ab_distance(a_lat, a_lng, b_lat, b_lng):
    center_lat = .5 * a_lat + .5 * b_lat
    lat_dis = lat_degree2km(abs(a_lat - b_lat))
    lng_dis = lng_degree2km(abs(a_lng - b_lng), center_lat)
    return math.sqrt(lat_dis ** 2 + lng_dis ** 2)

def start(x,y,z):

    name,num=bys(x,y,z)
    if len(name)==0:
        print('没有放球站能到达')
        return
    t=0
    for i in range(6):
        if num[i]<0.01:
            break
        sin,cos=RNN(name[i])
        if sin==1 and cos==1:
            continue
        a = math.atan2(sin, cos)
        b = math.atan2(y-stations[name[i]][1], x-stations[name[i]][0])
        l=ab_distance(y, x, stations[name[i]][1], stations[name[i]][0])
        if l<z:
            print(name[i]+'在敏感区范围内')
            t+=1
            continue
        c=math.asin(z/l)
        d=abs(a-b)
        if d > math.pi:
            d=2*math.pi-d
        if d < c:
            if num[i] > 0.5:
                with open('E:\VSProjects\\test\data.txt', 'w') as f:  # 设置文件对象
                    f.write(name[i]+'有较大可能')
                print(name[i]+'有较大可能')
                t+=1
            else:
                with open('E:\VSProjects\\test\data.txt', 'w') as f:  # 设置文件对象
                    f.write(name[i]+'有可能')
                print(name[i]+'有可能')
                t+=1
        elif d < 2*c:
            with open('E:\VSProjects\\test\data.txt', 'w') as f:  # 设置文件对象
                f.write(name[i]+'有可能')
            print(name[i]+'有可能')
            t+=1
    if t==0:
        with open('E:\VSProjects\\test\data.txt', 'w') as f:  # 设置文件对象
            f.write('没有放球站能到达敏感区')
        print('没有放球站能到达敏感区')

if __name__=='__main__':
    start(float(sys.argv[1]),float(sys.argv[2]),float(sys.argv[3])*111)
