public var startNode : boolean = false; // lets us know if this is the starting node
public var endNode: boolean = false; // lets us know if this is the ending node.
public var visitingNode:boolean = false; // lets us know we are at the node
public var visitedNode: boolean = false; // lets us know we visited the node previously
public var resetRoute:boolean  = false; // lets us know to reset the route and erase visit flags.

function Update()
{
	if (resetRoute == true)
	{
		if (startNode == true)
		{
			endNode = true;
			startNode = false;
			visitingNode = false;
			visitedNode = false;
		}
		else if (endNode == true)
		{
			endNode = false;
			startNode = true;
			visitingNode = false;
			visitedNode = false;
		}
		else
		{
			visitedNode = false;
			visitingNode = false;
		}
		resetRoute = false;
	}
}