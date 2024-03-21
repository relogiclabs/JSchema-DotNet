namespace RelogicLabs.JSchema.Tests.Positive;

// Use native functions for large and process-intensive tasks
[TestClass]
public class ScriptSortTests
{
    [TestMethod]
    public void When_SortArrayUsingMergeSort_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "sorted": #array &sorted,
                "unsorted": @mergeSort(&sorted) #array
            }
            %script: {
                constraint mergeSort(result) {
                    // Copy creates a writable copy from readonly node
                    var array = copy(target);
                    mergeSort(array, 0, size(target) - 1);
                    if(array != result[0]) return fail("Invalid: " + target);
                }
                
                subroutine mergeSort(array, low, high) {
                    if(low < high) {
                        var mid = (low + high) / 2;
                        mergeSort(array, low, mid);
                        mergeSort(array, mid + 1, high);
                        merge(array, low, mid, high);
                    }
                }
                
                subroutine merge(array, low, mid, high) {
                    var n1 = mid - low + 1;
                    var n2 = high - mid;
                    var left = [];
                    var right = [];
                    
                    for(var i = 0; i < n1; i++) {
                        left[i] = array[low + i];
                    }
                    
                    for(var j = 0; j < n2; j++) {
                        right[j] = array[mid + 1 + j];
                    }
                    
                    var i = 0, j = 0, k = low;
                    while(i < n1 && j < n2) {
                        if(left[i] <= right[j]) {
                            array[k++] = left[i++];
                        } else {
                            array[k++] = right[j++];
                        }
                    }
                    while(i < n1) array[k++] = left[i++];
                    while(j < n2) array[k++] = right[j++];
                }
            }
            """;
        var json =
            """
            {
                "sorted": [26.73, 27.64, 30.10, 43.23, 49.67, 78.75, 118.82, 143.45, 163.78, 
                    174.04, 187.14, 191.31, 203.42, 217.67, 242.93, 243.58, 245.01, 264.11, 
                    264.30, 267.15, 275.08, 290.28, 293.32, 293.81, 321.01, 365.10, 367.10, 
                    377.74, 404.28, 422.42, 427.30, 433.56, 435.14, 442.65, 454.49, 458.71, 
                    462.34, 469.23, 478.34, 479.23, 495.34, 519.93, 524.16, 524.77, 526.23, 
                    532.42, 539.10, 564.29, 583.18, 585.50],

                "unsorted": [454.49, 293.32, 143.45, 519.93, 532.42, 78.75, 267.15, 43.23, 427.30, 
                    245.01, 433.56, 30.10, 583.18, 404.28, 275.08, 49.67, 203.42, 191.31, 174.04, 
                    524.77, 495.34, 163.78, 264.30, 479.23, 365.10, 290.28, 118.82, 458.71, 187.14, 
                    264.11, 243.58, 526.23, 422.42, 442.65, 469.23, 217.67, 564.29, 585.50, 27.64, 
                    435.14, 26.73, 293.81, 478.34, 462.34, 367.10, 321.01, 242.93, 524.16, 377.74, 
                    539.10]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_SortArrayUsingQuickSort_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "sorted": #array &sorted,
                "unsorted": @quickSort(&sorted) #array
            }
            %script: {
                constraint quickSort(result) {
                    var array = copy(target);
                    quickSort(array, 0, size(target) - 1);
                    if(array != result[0]) return fail("Invalid: " + target);
                }
                
                subroutine quickSort(array, low, high) {
                    if(low < high) {
                        var pi = partition(array, low, high);
                        quickSort(array, low, pi - 1);
                        quickSort(array, pi + 1, high);
                    }
                }
                
                subroutine partition(array, low, high) {
                    var pivot = array[high];
                    var i = low - 1;
                    for(var j = low; j < high; j++) {
                        if(array[j] <= pivot) {
                            swap(array, ++i, j);
                        }
                    }
                    swap(array, i + 1, high);
                    return i + 1;
                }
                
                subroutine swap(array, i, j) {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
            """;
        var json =
            """
            {
                "sorted": [26.73, 27.64, 30.10, 43.23, 49.67, 78.75, 118.82, 143.45, 163.78, 
                    174.04, 187.14, 191.31, 203.42, 217.67, 242.93, 243.58, 245.01, 264.11, 
                    264.30, 267.15, 275.08, 290.28, 293.32, 293.81, 321.01, 365.10, 367.10, 
                    377.74, 404.28, 422.42, 427.30, 433.56, 435.14, 442.65, 454.49, 458.71, 
                    462.34, 469.23, 478.34, 479.23, 495.34, 519.93, 524.16, 524.77, 526.23, 
                    532.42, 539.10, 564.29, 583.18, 585.50],

                "unsorted": [564.29, 462.34, 143.45, 118.82, 290.28, 242.93, 435.14, 458.71, 539.10, 
                    585.50, 524.16, 78.75, 243.58, 404.28, 217.67, 264.30, 367.10, 49.67, 203.42, 
                    26.73, 479.23, 245.01, 264.11, 532.42, 524.77, 583.18, 469.23, 27.64, 275.08, 
                    442.65, 454.49, 377.74, 187.14, 267.15, 163.78, 519.93, 174.04, 191.31, 321.01, 
                    427.30, 293.32, 30.10, 478.34, 526.23, 365.10, 43.23, 433.56, 422.42, 293.81, 
                    495.34]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_SortArrayUsingInsertionSort_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "unsorted": @insertionSort(&sorted) #array,
                "sorted": #array &sorted
            }
            %script: {
                future insertionSort(result) {
                    var array = copy(target);
                    var n = size(target);
                    for(var i = 1; i < n; i++) {
                        var key = array[i];
                        var j = i - 1;
                        while(j >= 0 && array[j] > key) {
                            array[j-- + 1] = array[j];
                        }
                        array[j + 1] = key;
                    }
                    if(array != result[0]) return fail("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "unsorted": [-284, -782, -162, -737, 159, -580, -678, -92, -416, -408, -460, 605, -143, 
                    808, -142, -869, -735, 984, -665, 514, -421, 668, -872, -647, -581, 574, -499, -956, 
                    204, 646, -382, 248, -597, 998, -590, -815, -969, -515, -970, 888, 972, -942, 690, 
                    981, 742, -409, 248, 599, 275, -325],
                    
                "sorted": [-970, -969, -956, -942, -872, -869, -815, -782, -737, -735, -678, -665, -647, 
                    -597, -590, -581, -580, -515, -499, -460, -421, -416, -409, -408, -382, -325, -284, 
                    -162, -143, -142, -92, 159, 204, 248, 248, 275, 514, 574, 599, 605, 646, 668, 690, 
                    742, 808, 888, 972, 981, 984, 998]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_SortArrayUsingBubbleSort_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "unsorted": @bubbleSort(&sorted) #array,
                "sorted": #array &sorted
            }
            %script: {
                future bubbleSort(result) {
                    var array = copy(target);
                    var n = size(target) - 1;
                    var swapped = false;
                
                    for(var i = 0; i < n; i++) {
                        swapped = false;
                        for(var j = 0; j < n - i; j++) {
                            if(array[j] > array[j + 1]) {
                                var temp = array[j];
                                array[j] = array[j + 1];
                                array[j + 1] = temp;
                                swapped = true;
                            }
                        }
                        if(!swapped) break;
                    }
                    if(array != result[0]) return fail("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "unsorted": [575, 991, 877, 106, 163, -149, 730, -604, 755, -125, 496, 39, -226, 
                    -803, 141, -461, -706, 390, -367, 631, 723, 789, -295, -615, 415, -150, 408, 
                    -684, 385, -984, -558, -990, -979, -129, 508, -529, 666, -891, -63, -715, -435, 
                    431, 135, -243, 787, -911, -475, 914, -970, -138],
                    
                "sorted": [-990, -984, -979, -970, -911, -891, -803, -715, -706, -684, -615, -604, 
                    -558, -529, -475, -461, -435, -367, -295, -243, -226, -150, -149, -138, -129, 
                    -125, -63, 39, 106, 135, 141, 163, 385, 390, 408, 415, 431, 496, 508, 575, 631, 
                    666, 723, 730, 755, 787, 789, 877, 914, 991]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_SortArrayUsingHeapSort_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "sorted": #array &sorted,
                "unsorted": @heapSort(&sorted) #array
            }
            %script: {
                constraint heapSort(result) {
                    var array = copy(target);
                    var n = size(array);
                
                    for(var i = n / 2 - 1; i >= 0; i--) {
                        heapify(array, n, i);
                    }
                
                    for(var i = n - 1; i >= 0; i--) {
                        var temp = array[0];
                        array[0] = array[i];
                        array[i] = temp;
                        heapify(array, i, 0);
                    }
                    if(array != result[0]) return fail("Invalid: " + target);
                }
                
                subroutine heapify(array, n, i) {
                    var largest = i;
                    var l = 2 * i + 1;
                    var r = 2 * i + 2;
                
                    if(l < n && array[l] > array[largest]) largest = l;
                    if(r < n && array[r] > array[largest]) largest = r;
                
                    if(largest != i) {
                        var swap = array[i];
                        array[i] = array[largest];
                        array[largest] = swap;
                        heapify(array, n, largest);
                    }
                }
            }
            """;
        var json =
            """
            {
                "sorted": [26.73, 27.64, 30.10, 43.23, 49.67, 78.75, 118.82, 143.45, 163.78, 
                    174.04, 187.14, 191.31, 203.42, 217.67, 242.93, 243.58, 245.01, 264.11, 
                    264.30, 267.15, 275.08, 290.28, 293.32, 293.81, 321.01, 365.10, 367.10, 
                    377.74, 404.28, 422.42, 427.30, 433.56, 435.14, 442.65, 454.49, 458.71, 
                    462.34, 469.23, 478.34, 479.23, 495.34, 519.93, 524.16, 524.77, 526.23, 
                    532.42, 539.10, 564.29, 583.18, 585.50],

                "unsorted": [454.49, 293.32, 143.45, 519.93, 532.42, 78.75, 267.15, 43.23, 427.30, 
                    245.01, 433.56, 30.10, 583.18, 404.28, 275.08, 49.67, 203.42, 191.31, 174.04, 
                    524.77, 495.34, 163.78, 264.30, 479.23, 365.10, 290.28, 118.82, 458.71, 187.14, 
                    264.11, 243.58, 526.23, 422.42, 442.65, 469.23, 217.67, 564.29, 585.50, 27.64, 
                    435.14, 26.73, 293.81, 478.34, 462.34, 367.10, 321.01, 242.93, 524.16, 377.74, 
                    539.10]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_SortArrayUsingCountingSort_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "unsorted": @countingSort(&sorted) #array,
                "sorted": #array &sorted
            }
            %script: {
                future countingSort(result) {
                    var array = copy(target);
                    countingSort(array);
                    if(array != result[0]) return fail("Invalid: " + target);
                }
                
                subroutine countingSort(array) {
                    var max = array[0];
                    var min = array[0];
                
                    foreach(var value in array) {
                        if (value > max) max = value;
                        if (value < min) min = value;
                    }
                
                    var range = max - min + 1;
                    var counts = fill(0, range);
                
                    foreach(var value in array) counts[value - min]++;
                
                    var index = 0;
                    for(var i = 0; i < range; i++) {
                        while(counts[i] > 0) {
                            array[index] = i + min;
                            index++;
                            counts[i]--;
                        }
                    }
                }
            }
            """;
        var json =
            """
            {
                "unsorted": [575, 991, 877, 106, 163, -149, 730, -604, 755, -125, 496, 39, -226, 
                    -803, 141, -461, -706, 390, -367, 631, 723, 789, -295, -615, 415, -150, 408, 
                    -684, 385, -984, -558, -990, -979, -129, 508, -529, 666, -891, -63, -715, -435, 
                    431, 135, -243, 787, -911, -475, 914, -970, -138],
                    
                "sorted": [-990, -984, -979, -970, -911, -891, -803, -715, -706, -684, -615, -604, 
                    -558, -529, -475, -461, -435, -367, -295, -243, -226, -150, -149, -138, -129, 
                    -125, -63, 39, 106, 135, 141, 163, 385, 390, 408, 415, 431, 496, 508, 575, 631, 
                    666, 723, 730, 755, 787, 789, 877, 914, 991]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_SortArrayUsingRadixSort_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "unsorted": @radixSort(&sorted) #array,
                "sorted": #array &sorted
            }
            %script: {
                future radixSort(result) {
                    var array = copy(target);
                    radixSort(array);
                    if(array != result[0]) return fail("Invalid: " + target);
                }
                
                subroutine radixSort(array) {
                    var max = array[0];
                    foreach(var num in array) {
                        if(num > max) max = num;
                    }
                
                    var numDigits = floor(log(max) / log(10)) + 1;
                    for(var digit = 1; digit <= numDigits; digit++) {
                        var buckets = [];
                        for(var i = 0; i < 10; i++) buckets[i] = [];
                        
                        foreach(var num in array) {
                            var bucket = buckets[floor(mod(num / pow(10, digit - 1), 10))];
                            bucket[size(bucket)] = num;
                        }
                        var index = 0;
                        foreach(var bucket in buckets) {
                            foreach(var num in bucket) {
                                array[index++] = num;
                            }
                        }
                    }
                }
            }
            """;
        var json =
            """
            {
                "unsorted": [133, 955, 254, 784, 645, 778, 679, 970, 807, 207, 407, 299, 260, 358, 134, 
                    374, 156, 840, 39, 344, 418, 309, 186, 314, 957, 182, 116, 741, 107, 636, 969, 527, 
                    61, 343, 96, 738, 538, 542, 244, 480, 617, 914, 532, 355, 544, 251, 669, 777, 610, 
                    947],
                "sorted": [39, 61, 96, 107, 116, 133, 134, 156, 182, 186, 207, 244, 251, 254, 260, 299, 
                    309, 314, 343, 344, 355, 358, 374, 407, 418, 480, 527, 532, 538, 542, 544, 610, 617, 
                    636, 645, 669, 679, 738, 741, 777, 778, 784, 807, 840, 914, 947, 955, 957, 969, 970]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
}