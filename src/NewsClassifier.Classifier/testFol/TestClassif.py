import sys
import io
import pandas as pd
import re
from sklearn.svm import SVC
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.preprocessing import LabelEncoder
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')

class Stemmer:
    def __init__(self):
        self.vowel = r'аеиоуюяіїє'
        self.perfectiveground = r'(ив|ивши|ившись|((?<=[ая])(в|вши|вшись)))$'
        self.reflexive = r'(с[яьи])$'
        self.adjective = (
            r'(ими|ій|ий|а|е|ова|ове|ів|їй|єє|еє|я|ім|ем|им|их|іх|ою|йми|ими|у|ю|ого|ому|ої)$'
        )
        self.participle = r'(ий|ого|ому|им|ім|а|ій|у|ою|ій|і|их|йми|их)$'
        self.verb = r'(сь|ся|ив|ать|ять|у|ю|ав|али|учи|ячи|вши|ши|е|ме|ати|яти|є)$'
        self.noun = (
            r'(а|ев|ов|е|ями|ами|еи|и|ей|ой|ий|й|иям|ям|ием|ем|ам|ом|о|у|ах|иях|ях|ы|ь|ию|ью|ю|ия|ья|я|і|ові|ї|ею|єю|ою|є|еві|ем|єм|ів|їв|ю)$'
        )
        self.rvre = r'[аеиоуюяіїє]'
        self.derivational = r'[^аеиоуюяіїє][аеиоуюяіїє]+[^аеиоуюяіїє]+[аеиоуюяіїє].*(?<=о)сть?$'
        self.prefix = r'(пре|при|прі|об|роз|без|від|пред|через|перед|понад|між|над|під|од)'

    def s(self, st, reg, to):
        orig = st
        self.RV = re.sub(reg, to, st)
        return orig != self.RV

    def stem_word(self, word):
        word = word.replace("'", "")
        word = word.lower()
        if not re.search(self.rvre, word):
            stem = word
        else:
            p = re.search(self.rvre, word)
            start = word[:p.span()[1]]
            self.RV = word[p.span()[1]:]
            if not self.s(self.RV, self.perfectiveground, ''):
                self.s(self.RV, self.reflexive, '')
                if self.s(self.RV, self.adjective, ''):
                    self.s(self.RV, self.participle, '')
                else:
                    if not self.s(self.RV, self.verb, ''):
                        self.s(self.RV, self.noun, '')
            self.s(self.RV, 'и$', '')
            if re.search(self.derivational, self.RV):
                self.s(self.RV, 'ость$', '')
            if self.s(self.RV, 'ь$', ''):
                self.s(self.RV, 'ейше?$', '')
                self.s(self.RV, 'нн$', 'н')
            stem = start + self.RV
            stem = ''.join([i for i in stem if not i.isdigit()])
        return stem

class NewsClassifier:
    def __init__(self, file_path, use_stemming=False):
        self.file_path = file_path
        self.vectorizer = TfidfVectorizer()
        self.label_encoder = LabelEncoder()
        self.svm_rbf = SVC(kernel='rbf')
        self.use_stemming = use_stemming

    def load_data(self):
        data = pd.read_csv(self.file_path, sep=':', encoding='utf-8', names=['Label', 'Text'])
        return data

    def preprocess_text(self, text):
        text = text.lower()
        text = re.sub(r'[^\sа-щьюяіїєґ]', '', text)
        return text

    def stem_text(self, text):
        stemmer = Stemmer()
        text = text.lower()
        words = re.split(r'(\W)', text)
        words = [word for word in words if word != '']
        for i in range(len(words)):
            words[i] = stemmer.stem_word(words[i])
        return ''.join(words)

    def preprocess_data(self, data):
        data['Text'] = data['Text'].apply(self.preprocess_text)
        if self.use_stemming:
            data['Text'] = data['Text'].apply(self.stem_text)
        data[['Label', 'Text']].to_csv(stemmed_text_file_path, sep=':', index=False, encoding='utf-8')
        return data

    def train(self, X_train, y_train):
        self.svm_rbf.fit(X_train, y_train)

    def classify_news(self, news_text):
        preprocessed_text = self.preprocess_text(news_text)
        if self.use_stemming:
            preprocessed_text = self.stem_text(preprocessed_text)
        X_test = self.vectorizer.transform([preprocessed_text])
        predicted_label_index = self.svm_rbf.predict(X_test)[0]
        predicted_label = self.label_encoder.inverse_transform([predicted_label_index])[0]
        return predicted_label

    def get_class_counts(self, data):
        class_counts = data['Label'].value_counts()
        print("Class Counts:")
        for label, count in class_counts.items():
            print(f"{label}: {count}")

file_path = r'C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\testNews.csv'
stemmed_text_file_path = r'C:\Users\sie29\source\repos\NewsClassifier\src\NewsClassifier.Classifier\testFol\stemmed_texts.csv'

# Классификатор без стеминга
classifier_no_stemming = NewsClassifier(file_path, use_stemming=False)
data_no_stemming = classifier_no_stemming.load_data()
classifier_no_stemming.get_class_counts(data_no_stemming)
preprocessed_data_no_stemming = classifier_no_stemming.preprocess_data(data_no_stemming)

X_no_stemming = preprocessed_data_no_stemming['Text']
y_no_stemming = preprocessed_data_no_stemming['Label']
X_train_no_stemming, X_test_no_stemming, y_train_no_stemming, y_test_no_stemming = train_test_split(X_no_stemming, y_no_stemming, test_size=0.2, random_state=42)

X_train_vectorized_no_stemming = classifier_no_stemming.vectorizer.fit_transform(X_train_no_stemming)
X_test_vectorized_no_stemming = classifier_no_stemming.vectorizer.transform(X_test_no_stemming)

y_train_encoded_no_stemming = classifier_no_stemming.label_encoder.fit_transform(y_train_no_stemming)
y_test_encoded_no_stemming = classifier_no_stemming.label_encoder.transform(y_test_no_stemming)

classifier_no_stemming.train(X_train_vectorized_no_stemming, y_train_encoded_no_stemming)

y_pred_no_stemming = classifier_no_stemming.svm_rbf.predict(X_test_vectorized_no_stemming)

accuracy_no_stemming = accuracy_score(y_test_encoded_no_stemming, y_pred_no_stemming)
print(f"Accuracy without stemming: {accuracy_no_stemming}")

# Классификатор со стемингом
classifier_with_stemming = NewsClassifier(file_path, use_stemming=True)
data_with_stemming = classifier_with_stemming.load_data()
classifier_with_stemming.get_class_counts(data_with_stemming)
preprocessed_data_with_stemming = classifier_with_stemming.preprocess_data(data_with_stemming)

X_with_stemming = preprocessed_data_with_stemming['Text']
y_with_stemming = preprocessed_data_with_stemming['Label']
X_train_with_stemming, X_test_with_stemming, y_train_with_stemming, y_test_with_stemming = train_test_split(X_with_stemming, y_with_stemming, test_size=0.2, random_state=42)

X_train_vectorized_with_stemming = classifier_with_stemming.vectorizer.fit_transform(X_train_with_stemming)
X_test_vectorized_with_stemming = classifier_with_stemming.vectorizer.transform(X_test_with_stemming)

y_train_encoded_with_stemming = classifier_with_stemming.label_encoder.fit_transform(y_train_with_stemming)
y_test_encoded_with_stemming = classifier_with_stemming.label_encoder.transform(y_test_with_stemming)

classifier_with_stemming.train(X_train_vectorized_with_stemming, y_train_encoded_with_stemming)

y_pred_with_stemming = classifier_with_stemming.svm_rbf.predict(X_test_vectorized_with_stemming)

accuracy_with_stemming = accuracy_score(y_test_encoded_with_stemming, y_pred_with_stemming)
print(f"Accuracy with stemming: {accuracy_with_stemming}")


def get_classify_news_label(news_text, use_stemming):
    if use_stemming:
        result = classifier_with_stemming.classify_news(news_text)
    else:
        result = classifier_no_stemming.classify_news(news_text)
    print(result)
    return result
