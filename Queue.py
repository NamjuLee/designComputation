import copy
from collections import deque

class Queue:
    data = []
    def __init__(self):
        self.data = []
    def push(self, val):
    	if(type(val)==list):
    		for i in val: self.data.append(i)
    	else: self.data.append(val)
    def pop(self):
        if(len(self.data) == 0 ):
            print ("no data remaining")
            return None
        else:return self.data.pop(0)
    def __eq__(self, other):
        if(len(self.data) != len(other.data)):
        	return False
        else:
            num = 0
            for i in range(len(self.data)):
                if(self.data[i] == other.data[i]):
                    num += 1;
            if(num == len(self.data)): return True;
            else: return False
    def __ne__(self, other):
        if(self == other):return False
        else:return True
    def __str__(self):
        theStr = ""
        for i in self.data:
            theStr += str(i)
        return theStr

theQueue = Queue()
theList = [1,2,3,4]
otherList = [1,2,3,4, 5]
num = 5

theQueue.push(theList)
theQueue.push(num)
print theQueue

otherQueue = Queue()
otherQueue.push(otherList)
print otherQueue

theQueue.pop()
otherQueue.pop()

print theQueue == otherQueue 

print theQueue
print otherQueue