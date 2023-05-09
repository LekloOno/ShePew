using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPhysics : MonoBehaviour
{
    //isGrounded
    static float groundedMaxHeight = .15f;

    //SuperJumpAngle
    static float sjAngleMaxHeight = 100f;
    static float sjAngleByVel = 0.3f;

    //SuperJumpVelBoost
    static float sjVelBottomStrength = 0.18f;
    static float sjVelBottomTightness = 18f;
    static float sjVelBottomOffset = -39f;

    static float sjVelTopTension = 2.8f;
    static float sjVelTopOffset = 16.0f;

    static float sjVelMaxTension = 0.34f;
    static float sjVelMaxFactor = 0.12f;

    static float sjVelHeightTension = 10f;
    static float sjVelHeightOffset = 9;

    static public Vector3 Acceleration(float maxSpeedBase, float maxAccelBase, Vector3 velocity, Vector3 direction, Vector3 gDirection)
    {
    /*  Returns the Accelerated Vector to be added to the current velocity vector of an entity.
    */
        float currentSpeed = Vector3.Dot(velocity, direction);
        float accel = Mathf.Max(Mathf.Min(maxSpeedBase - currentSpeed, maxAccelBase*maxSpeedBase*Time.fixedDeltaTime),0);
        return accel * gDirection;
    }

    static public float MaxAccelAngle(float velocity, float maxSpeed, float maxAccel, float accuracy=0.5f){
        float angle = accuracy;
        float prev = AccelForAngle(velocity, maxSpeed, maxAccel, 0);
        float current = AccelForAngle(velocity, maxSpeed, maxAccel, angle); 
        while(angle < 90 && prev <= current){
            angle += accuracy;
            prev = current;
            current = AccelForAngle(velocity, maxSpeed, maxAccel, angle);
        }

        return angle;
    }

    static public float AccelForAngle(float velocity, float maxSpeed, float maxAccel, float angle){
        return Mathf.Min(Mathf.Max(maxSpeed - velocity*Mathf.Cos(Mathf.Deg2Rad*angle),0), maxSpeed*maxAccel*Time.fixedDeltaTime)*velocity*Mathf.Cos(Mathf.Deg2Rad*angle);
    }

    static public bool isGrounded(Transform obj, float objHeight, LayerMask ground)
    {
    /*  Returns True if the player is grounded.......... WOW I COULDNT TELL */
        return Physics.Raycast(obj.position, Vector3.down, obj.localScale.y * objHeight * 0.5f + groundedMaxHeight, ground);
    }

    static public Vector3 SuperJumpVecForce(Vector3 fwdDir, float velocity, float height, float slideYScale)
    {
    /*  Returns the Vector used to emulate the Super Jump, both intensity and direction-wise.
    (Angle are represented as y vector magnitude | 0 <= x <= 1)

    _uses :
    SuperJumpVelBoost()
    SuperJumpAngle()
    VectorWithThisY()

    */
        Vector3 forceDir = VectorWithThisY(fwdDir, SuperJumpAngle(fwdDir.y, velocity, height));
        float velMultiplier = 1f+SuperJumpVelBoost(velocity, height, slideYScale);
        
        return forceDir*velMultiplier*8f;
    }

    static public float SuperJumpAngle(float angle, float velocity, float height)   
    {
    /*  Returns the SuperJump Angle, using the camera angle of the entity, its speed, and its distance from ground.
    (Angle are represented as y vector magnitude | 0 <= x <= 1)

    f(x,v,h) = ((x/H-1/H)*min(H,h)+1)*e^((1.9*x-1.9)/(e^(V*v)+1))^(1-min(H,h)/H))
    https://www.desmos.com/calculator/xbhkziu2lr

    _variables :
    x - angle   - entity camera direction angle,
    v - velocity- entity velocity,
    h - height  - entity distance from ground.

    _constants :
    H - sjAngleMaxHeight = 10.0  - h=H, ⇒ f(x,v,h) = x                  - The height at which the player has full control on the superJump direction.
    V - sjAngleByVel     =  0.3 - V=0, ⇒ f(x,v,h) = f(x,k,h) ∀k ∈ ℝ     - How much the velocity impacts the angle. The closest to zero, the less it will impact it.

    */
        return ((angle/sjAngleMaxHeight-1/sjAngleMaxHeight)*Mathf.Min(sjAngleMaxHeight, height)+1)*Mathf.Pow(Mathf.Exp((1.9f*angle-1.9f)/(Mathf.Exp(sjAngleByVel*velocity)+1)),1-Mathf.Min(sjAngleMaxHeight, height)/sjAngleMaxHeight);
    }

    static public float SuperJumpVelBoost(float velocity, float height, float slideYScale)
    {
    /*  Returns the SuperJump force strength, depending on the initial speed and height.

    f(x) = (E*max(-(ax+b)^3-(0.7*(ax+b))^4,0)+I*max(x-c,0)^i+min((x/c)^s,1))/(t^(min(max(h,y),H)-o)+1)
    https://www.desmos.com/calculator/josvtxjbbn

    _variables :
    x - velocity    - the entity velocity.
    h - height      - the entity distance from ground.
    y - slideYscale - The entity body Y scale when sliding.
    
    _constants :
    Bottom Edge boost       -   tweak the behavior of the function on the bottom edge boost. (right before the slide gets cancelled)
    E - sjVelBottomStrength =  0.18 - intensity of the edge boost, 0 means it has no effect.
    a - sjVelBottomTightness=  6.5  - tightness/margin of error of the edge boost. The higher _a_ is, the smaller the margin of error and therefore the harder the edge boost to perform.
    b - sjVelBottomOffset   = -7.0  - change the spike position of the boost.

    Until high speed spike  -   tweak the behavior of the function until the higher speed spike, before the function starts flattenning.   
    s - sjVelTopTension     =  3.0  - how exponential is the function until the high speed spike. 1 means it's linear.
    c - sjVelTopOffset      = 16.0  - The speed required to achieve the spike.

    Beyond speed spike      -   tweak the behavior of the function on and beyond the high speed spike.
    i - sjVelMaxTension     =  0.34 - Tension from the spike. 1 means it's linear. The closer it is to zero, the sharper is the spike.
    I - sjVelMaxFactor      =  0.12 - Factor of the speed beyond the spike.

    Height Debuff           -   tweak the intensity of the height Debuff (limit the speed when in the air, to balance the fact that the velocity is spread more horizontally the higher the SJ is performed.)
    H - sjAngleMaxHeight    = 10.0  - The height at which the debuff is capped.
    t - sjVelHeightTension  =  1.6  - The tension of the debuff, tweak the desmos for better understanding.
    o - sjVelHeightOffset   = 10.0  - The offset of the tension, tweak the desmos for better understanding.
 
    */
        float heightDebuff = Mathf.Pow(sjVelHeightTension,Mathf.Clamp(height, slideYScale, sjAngleMaxHeight)-sjVelHeightOffset)+0.5f;
        return 0.7f*(sjVelBottomStrength*Mathf.Max(-Mathf.Pow(sjVelBottomTightness*velocity+sjVelBottomOffset,3)-Mathf.Pow(0.7f*(sjVelBottomTightness*velocity+sjVelBottomOffset),4),0)+sjVelMaxFactor*Mathf.Pow(Mathf.Max(velocity-sjVelTopOffset,0),sjVelMaxTension)+Mathf.Min(Mathf.Pow((velocity/sjVelTopOffset),sjVelTopTension),1))/heightDebuff;
    }

    static public Vector3 VectorWithThisY(Vector3 inputVec, float yVec)
    {
    /* Returns a new Vector(inputVec.x*k, yVec, inputVect.z*k) with k a constant so that the vector is normalised.
    (Angle are represented as y vector magnitude | 0 <= x <= 1)

    k(x,X,Z) = (1-x)/sqrt(-(x-1)*(X^2+Z^2))
    https://www.desmos.com/calculator/qucz2aqsic

    _variables :
    x - yVec    - the value we want as the y parameter of the output vector.
    X - inputVec.x
    Z - inputVec.z

    _constants :
    -none-

    */
        float k = 0f;
        if(inputVec.x != 0 || inputVec.z != 0) {
            k = (1-yVec)/Mathf.Sqrt(-(yVec-1)*(Mathf.Pow(inputVec.x,2)+Mathf.Pow(inputVec.z,2)));
        }
        return new Vector3(inputVec.x*k, yVec, inputVec.z*k);
    }

    static public float SlopeSlideSpeed(float velocity, float maxSpeed)
    {
    /*
    https://www.desmos.com/calculator/is80e2nycl
    */
        return Mathf.Log(Mathf.Max(velocity, maxSpeed)+1-maxSpeed,1.8f)+6.2f;
    }
}