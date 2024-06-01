# coding: utf-8
import sys
import io
import pandas as pd
import re
from uk_stemmer import UkStemmer
from sklearn.svm import SVC
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.preprocessing import LabelEncoder

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

class NewsClassifier:
    def __init__(self, file_path):
        self.file_path = file_path
        self.vectorizer = TfidfVectorizer()
        self.label_encoder = LabelEncoder()
        self.svm_rbf = SVC(kernel='rbf')

    def load_data(self):
        data = pd.read_csv(self.file_path, sep=':', encoding='utf-8', names=['Label', 'Text'])
        return data

    def preprocess_text(self, text):
        text = text.lower()
        text = re.sub(r'[^\sа-щьюяіїєґ]', '', text)
        return text

    def stem_text(self, text):
        stemmer = UkStemmer()
        text = text.lower()
        words = re.split(r'(\W)', text)
        words = [word for word in words if word != '']
        for i in range(len(words)):
            words[i] = stemmer.stem_word(words[i])
        return ''.join(words)

    def preprocess_data(self, data):
        data['Text'] = data['Text'].apply(self.preprocess_text)
        data['Text'] = data['Text'].apply(self.stem_text)
        data[['Label', 'Text']].to_csv(stemmed_text_file_path, sep=':', index=False, encoding='utf-8')
        return data

    def train(self, train_data):
        X_train = self.vectorizer.fit_transform(train_data['Text'])
        y_train = self.label_encoder.fit_transform(train_data['Label'])
        self.svm_rbf.fit(X_train, y_train)

    def classify_news(self, news_text):
        preprocessed_text = self.preprocess_text(news_text)
        stemmed_text = self.stem_text(preprocessed_text)
        X_test = self.vectorizer.transform([stemmed_text])
        predicted_label_index = self.svm_rbf.predict(X_test)[0]
        predicted_label = self.label_encoder.inverse_transform([predicted_label_index])[0]
        return predicted_label

file_path = r'C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\testNews.csv'
stemmed_text_file_path = r'C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\stemmed_texts.csv'
classifier = NewsClassifier(file_path)
data = classifier.load_data()
processed_data = classifier.preprocess_data(data)
train_data = processed_data.sample(frac=0.8, random_state=2)
test_data = processed_data.drop(train_data.index)
classifier.train(train_data)

def get_classify_news_label(news_text):
    result = classifier.classify_news(news_text)
    print(result)
    return result

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: classify_and_exit.py <news_text>")
        sys.exit(1)

    news_text = sys.argv[1]
    get_classify_news_label(news_text)


