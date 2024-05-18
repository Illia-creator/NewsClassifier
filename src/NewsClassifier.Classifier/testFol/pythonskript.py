import os
import numpy as np
import pandas as pd

def calculateAccuracy():
    data = pd.read_csv(r'C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\iris.csv')
    train_data = data.sample(frac=0.9, random_state=1)
    test_data = data.drop(train_data.index)
    predictions = predict(train_data, test_data)
    accuracy = sum(predictions == test_data['variety']) / len(test_data)
    return accuracy
#print(f'Accuracy: {accuracy}')
#print(f'Test {test_data}')

def modified_distance(x1, x2): #���������� ������������ �������� ������� �� ����� ��������
    return np.sqrt(np.sum((x1 - x2)*(x1 - x2)))
def find_nearest_neighbor(train_data, test_sample):
    distances = []
    for _, train_sample in train_data.iterrows():
        distance = modified_distance(train_sample[:4], test_sample[:4])
        distances.append((distance, train_sample['variety']))
    distances.sort()
    nearest_neighbor = distances[0][1] # ������� ����������� ����� ��
    #  print(f'Test sample:  {test_sample[:4].values}, nearest neighbor: {nearest_neighbor}')
    return distances[0][1] #
def predict(train_data, test_data):
    predictions = []
    for _, test_sample in test_data.iterrows():
        nearest_neighbor = find_nearest_neighbor(train_data, test_sample)
        predictions.append(nearest_neighbor)
    return predictions
