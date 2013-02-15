#pragma strict
var counter = 0;
var TestTargetCounter =0;
var LookatCounter = 0;
var testtarget = "Target1";

var target : Transform;
var damping = 12.0;
var smooth = true;
var FishConstrainFreq = 5;

function Start () {

}


function FixedUpdate () {
//Swap the targets
ChangeTestTarget();

//Stop applying new force to Fish that are close to their targets
var dist = 0.00000;
dist = Vector3.Distance(GameObject.Find(testtarget).transform.position, transform.position);
if(dist > 1)
FishSwim();


//Counter for regulating the movement and rotation checks
counter = counter+1;
//How often to check the movement and rotation constaints
if(counter > FishConstrainFreq){
//Fish Movement Constaints
ConstrainPosition();
//Constrain Fish Rotations
ConstrainRotation();
}

}

//@script AddComponentMenu("Camera-Control/Smooth Look At")

function LateUpdate () {


	if (target) {
		if (smooth)
		{
			// Look at and dampen the rotation
			var rotation = Quaternion.LookRotation(target.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
		}
		else
		{
			// Just lookat
		    transform.LookAt(target);
		}
	}
	
	
}


function FishSwim(){

gameObject.rigidbody.AddRelativeForce(0,0,0.003,ForceMode.Impulse);
}

//This function constrains the fish within certain rotations
function ConstrainRotation(){
//Fish Rotational Constraints

//Local Y Axis
//No constraints

//Local x Axis allow a little wobble
if(gameObject.transform.eulerAngles.x > 10 && gameObject.transform.eulerAngles.x < 180)
gameObject.transform.eulerAngles.x = 10;

if(gameObject.transform.eulerAngles.x > 180 && gameObject.transform.eulerAngles.x <350)
gameObject.transform.eulerAngles.x = 350;


//Local Z Axis allow a little tilting back and forward
if(gameObject.transform.eulerAngles.z > 25 && gameObject.transform.eulerAngles.z < 180)
gameObject.transform.eulerAngles.z = 25;

if(gameObject.transform.eulerAngles.z > 180 && gameObject.transform.eulerAngles.z <335)
gameObject.transform.eulerAngles.z = 335;

counter = 0;
}



//This function constrains the fish within boundaries
function ConstrainPosition(){

//Global Z Axis Position
if(gameObject.transform.position.z > GameObject.Find("FishBoundaryBack").transform.position.z)
gameObject.transform.position.z = GameObject.Find("FishBoundaryBack").transform.position.z;

if(gameObject.transform.position.z < GameObject.Find("FishBoundaryFront").transform.position.z)
gameObject.transform.position.z = GameObject.Find("FishBoundaryFront").transform.position.z;

//Global X Axis Position
if(gameObject.transform.position.x > GameObject.Find("FishBoundaryRight").transform.position.x)
gameObject.transform.position.x = GameObject.Find("FishBoundaryRight").transform.position.x;

if(gameObject.transform.position.x < GameObject.Find("FishBoundaryLeft").transform.position.x)
gameObject.transform.position.x = GameObject.Find("FishBoundaryLeft").transform.position.x;

//Global Y Axis Position
if(gameObject.transform.position.y > GameObject.Find("FishBoundaryTop").transform.position.y)
gameObject.transform.position.y = GameObject.Find("FishBoundaryTop").transform.position.y;

if(gameObject.transform.position.y < GameObject.Find("FishBoundaryBottom").transform.position.y)
gameObject.transform.position.y = GameObject.Find("FishBoundaryBottom").transform.position.y;
}


//This function just switches through the test targets on a timer
function ChangeTestTarget(){

//TestTargetCounter update
TestTargetCounter = TestTargetCounter+1;

//Change Test Targets over time based on frames
if(TestTargetCounter < 3000)
testtarget = "Target3";

if(TestTargetCounter < 2000)
testtarget = "Target2";

if(TestTargetCounter < 1000)
testtarget = "Target1";

if (Input.GetButton("Fire1")){
Debug.Log(testtarget);
}

//Update Fish Direction counter
LookatCounter = LookatCounter+1;

//Update Fish Direction
if(LookatCounter == 60){
//gameObject.transform.LookAt(GameObject.Find(testtarget).transform);
target = GameObject.Find(testtarget).transform;
LookatCounter = 0;
}

if(TestTargetCounter > 3000)
TestTargetCounter = 0;

}