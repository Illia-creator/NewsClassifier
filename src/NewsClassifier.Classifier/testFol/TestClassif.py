import pandas as pd
import re
from uk_stemmer import UkStemmer
from sklearn.svm import SVC
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import classification_report
from pythonskript import get_classify_news_label
import sys

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: classify_and_exit.py <news_text>")
        sys.exit(1)

    news_text = sys.argv[1]
    result = get_classify_news_label(news_text)
    print(result)
    sys.exit(0)
