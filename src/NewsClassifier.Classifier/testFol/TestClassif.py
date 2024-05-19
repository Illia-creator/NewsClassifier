import pandas as pd
import numpy as np
import string
import re
from uk_stemmer import UkStemmer
from sklearn.svm import SVC
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.pipeline import make_pipeline
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import classification_report
from sklearn.svm import SVC
from datasets import load_dataset

file_path = r'C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\testNews.csv'
data = pd.read_csv(file_path, sep=':', encoding='utf-8', names=['Label', 'Text'])

label_counts = data['Label'].value_counts()

# Вывод количества записей для каждой метки в консоль
print("Количество записей для каждой метки:")
print(label_counts)

train_data = data.sample(frac=0.8, random_state=2)
test_data = data.drop(train_data.index)

def process_text(text):
    # Приведение к нижнему регистру
    text = text.lower()
    # Удаление всех символов, не являющихся украинскими буквами
    text = re.sub(r'[^\sа-щьюяіїєґ]', '', text)
    return text

train_data['Text'] = train_data['Text'].apply(process_text)
test_data['Text'] = test_data['Text'].apply(process_text)

stemmer = UkStemmer()

def stem_text(text):
    text = text.lower()
    words = re.split(r'(\W)', text)
    words = [word for word in words if word != '']
    for i in range(len(words)):
        words[i] = stemmer.stem_word(words[i])
    return ''.join(words)

train_data['Text'] = train_data['Text'].apply(stem_text)
test_data['Text'] = test_data['Text'].apply(stem_text)


# Переводим метки классов и текст новостей на украинский
class_translation = {
    'Війна': 'Війна',
    'Політика': 'Політика',
    'Економіка': 'Економіка',
    'Тактична інформація': 'Тактична інформація',
    'Наука': 'Наука',
    'Здоровя': 'Здоровя',
    'Погода': 'Погода',
    'Спорт': 'Спорт',
    'Культура': 'Культура',
    'Інциденти': 'Інциденти'
}

train_data['Label'] = train_data['Label'].map(class_translation)
test_data['Label'] = test_data['Label'].map(class_translation)

# Сохраняем обновленные данные в новые файлы CSV
train_data.to_csv(r'C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\train_data_stemmed.csv', index=False, sep=':')
test_data.to_csv(r'C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\test_data_stemmed.csv', index=False, sep=':')

# Выводим по пять строк из каждого набора данных для проверки
print("Обучающий набор данных:")
print(train_data.head(5))

print("\nТестовый набор данных:")
print(test_data.head(5))


# Инициализируем векторизатор TF-IDF
vectorizer = TfidfVectorizer()

# Преобразуем текстовые данные в матрицу TF-IDF признаков
X_train = vectorizer.fit_transform(train_data['Text'])
X_test = vectorizer.transform(test_data['Text'])

# Инициализируем кодировщик меток классов
label_encoder = LabelEncoder()

# Кодируем метки классов
y_train = label_encoder.fit_transform(train_data['Label'])
y_test = label_encoder.transform(test_data['Label'])

# Инициализируем SVM с ядром RBF
svm_rbf = SVC(kernel='rbf')

# Обучаем классификатор на обучающих данных
svm_rbf.fit(X_train, y_train)

# Предсказываем метки классов для тестовых данных
y_pred = svm_rbf.predict(X_test)

# Декодируем предсказанные метки классов
y_pred_decoded = label_encoder.inverse_transform(y_pred)

# Оцениваем качество классификации
print(classification_report(y_test, y_pred, target_names=label_encoder.classes_))
