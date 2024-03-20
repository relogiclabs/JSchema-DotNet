namespace RelogicLabs.JSchema.Tests.Positive;

// Use native functions for large and process-intensive tasks
[TestClass]
public class ScriptSearchTests
{
    [TestMethod]
    public void When_LinearSearchInString_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "mainText": @searchTest(&query) #string,
                "query": #string &query
            }
            %script: {
                future searchTest(query) {
                    var index = search(target, query[0]);
                    print("Received query: " + query[0]);
                    print("Found at: " + index);
                    if(index != 28) return fail("Invalid: " + target);
                }
                
                subroutine search(text, query) {
                    var n = size(text);
                    var m = size(query);
                    
                    for(var i = 0; i <= n - m; i++) {
                        var match = true;
                        for(var j = 0; j < m; j++) {
                            if(text[i + j] != query[j]) {
                                match = false;
                                break;
                            }
                        }
                        if(match) return i;
                    }
                    return -1;
                }
            }
            """;
        var json =
            """
            {
                "mainText": "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
                "query": "consectetur"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_BinarySearchInArray_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "sortedArray": @searchTest(&searchKey) #float* #array,
                "searchKey": #float &searchKey
            }
            %script: {
                future searchTest(searchKey) {
                    var index = binarySearch(target, searchKey[0]);
                    print("Received key: " + searchKey[0]);
                    print("Found at: " + index);
                    if(index != 97) return fail("Invalid: " + target);
                }
                
                subroutine binarySearch(array, key) {
                    var low = 0;
                    var high = size(array) - 1;
                
                    while(low <= high) {
                        var mid = low + (high - low) / 2;
                        var midVal = array[mid];
                
                        if(midVal == key) {
                            return mid;
                        } else if(midVal < key) {
                            low = mid + 1;
                        } else {
                            high = mid - 1;
                        }
                    }
                    return -1;
                }
            }
            """;
        var json =
            """
            {
                "sortedArray": [26.73, 27.64, 30.10, 43.23, 49.67, 78.75, 118.82, 143.45, 163.78, 
                    174.04, 187.14, 191.31, 203.42, 217.67, 242.93, 243.58, 245.01, 264.11, 
                    264.30, 267.15, 275.08, 290.28, 293.32, 293.81, 321.01, 365.10, 367.10, 
                    377.74, 404.28, 422.42, 427.30, 433.56, 435.14, 442.65, 454.49, 458.71, 
                    462.34, 469.23, 478.34, 479.23, 495.34, 519.93, 524.16, 524.77, 526.23, 
                    532.42, 539.10, 564.29, 583.18, 585.50, 614.79, 616.86, 620.88, 634.81, 
                    637.26, 639.96, 652.08, 657.21, 657.42, 659.09, 665.45, 681.13, 696.57, 
                    704.03, 713.98, 727.89, 734.30, 757.58, 761.33, 778.55, 781.93, 785.39, 
                    801.44, 812.91, 814.34, 824.60, 827.37, 834.94, 857.54, 865.48, 870.42, 
                    870.87, 879.25, 889.30, 899.27, 905.59, 915.16, 916.99, 930.06, 930.93, 
                    933.80, 935.46, 950.84, 957.79, 977.36, 977.45, 978.02, 980.75, 983.62, 
                    988.69],
                "searchKey": 980.75
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DepthFirstSearchInGraph_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "adjacencyList": @dfsTest(&visitedOrder) #array,
                "visitedOrder": #string &visitedOrder
            }
            %script: {
                var visitedOrder = "";
                future dfsTest(order) {
                    var graph = [[6, 7, 12], [2, 9], [1, 8], [4, 5], [3, 13], [3, 17], [0, 8], 
                        [0, 10, 18], [2, 6], [1, 14], [0, 11], [10, 15], [0, 13], [4, 12], [4], 
                        [9, 16], [11, 19], [15], [5, 19], [7]];
                    if(graph != target) return fail("Different graph: " + target);
                    var visited = fill(false, size(graph));
                    dfs(graph, 0, visited);
                    visitedOrder = visitedOrder[2..];
                    if(visitedOrder != order[0]) return fail("Invalid: " + order);
                }
                
                subroutine dfs(graph, start, visited) {
                    visited[start] = true;
                    visitedOrder = visitedOrder + ", " + start;
                    foreach(var neighbor in graph[start]) {
                        if(!visited[neighbor]) {
                            dfs(graph, neighbor, visited);
                        }
                    }
                }
            }
            """;
        var json =
            """
            {
                "adjacencyList": [[6, 7, 12], [2, 9], [1, 8], [4, 5], [3, 13], [3, 17], [0, 8], 
                        [0, 10, 18], [2, 6], [1, 14], [0, 11], [10, 15], [0, 13], [4, 12], [4], 
                        [9, 16], [11, 19], [15], [5, 19], [7]],
                "visitedOrder": "0, 6, 8, 2, 1, 9, 14, 4, 3, 5, 17, 15, 16, 11, 10, 19, 7, 18, 13, 12"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_BreadthFirstSearchInGraph_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "adjacencyList": @bfsTest(&visitedOrder) #array,
                "visitedOrder": #string &visitedOrder
            }
            %script: {
                var visitedOrder = "";
                
                future bfsTest(order) {
                    var graph = [[6, 7, 12], [2, 9], [1, 8], [4, 5], [3, 13], [3, 17], [0, 8], 
                        [0, 10, 18], [2, 6], [1, 14], [0, 11], [10, 15], [0, 13], [4, 12], [4], 
                        [9, 16], [11, 19], [15], [5, 19], [7]];
                    if(graph != target) return fail("Different graph: " + target);
                    bfs(graph, 0);
                    visitedOrder = visitedOrder[..-2];
                    if(visitedOrder != order[0]) return fail("Invalid: " + order);
                }
                
                subroutine bfs(graph, start) {
                    var visited = fill(false, size(graph));
                    var front = 0, rear = 0;
                    var queue = [];
                    queue[rear++] = start;
                    visited[start] = true;
                
                    while(front != rear) {
                        var current = queue[front++];
                        visitedOrder = visitedOrder + current + ", ";
                
                        foreach(var neighbor in graph[current]) {
                            if(!visited[neighbor]) {
                                queue[rear++] = neighbor;
                                visited[neighbor] = true;
                            }
                        }
                    }
                }
            }
            """;
        var json =
            """
            {
                "adjacencyList": [[6, 7, 12], [2, 9], [1, 8], [4, 5], [3, 13], [3, 17], [0, 8], 
                        [0, 10, 18], [2, 6], [1, 14], [0, 11], [10, 15], [0, 13], [4, 12], [4], 
                        [9, 16], [11, 19], [15], [5, 19], [7]],
                "visitedOrder": "0, 6, 7, 12, 8, 10, 18, 13, 2, 11, 5, 19, 4, 1, 15, 3, 17, 9, 16, 14"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
}