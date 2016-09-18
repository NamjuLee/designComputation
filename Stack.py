import copy
from collections import deque

class Stack:
    data = []
    def __init__(self):
        self.data = []
    def push(self, val):
    	if(type(val)==list):
    		for i in val:
    			self.data.append(i)
    	else:
    		self.data.append(val)
    def pop(self):
        if(len(self.data) == 0 ):
            print ("no data remaining")
            return None
        else:return self.data.pop(-1)
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


theStack = Stack()
theList = [1,2,3,4]
otherList = [1,2,3,4, 5]
num = 5

theStack.push(theList)
theStack.push(num)
print theStack

otherStack = Stack()
otherStack.push(otherList)
print otherStack

theStack.pop()
otherStack.pop()

print theStack == otherStack 

print theStack
print otherStack