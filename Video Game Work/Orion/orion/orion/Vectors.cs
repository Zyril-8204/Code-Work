using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace orion
{
    public class Vectors
    {
        const float rad2deg = 57.29577f;    //Converts radians to degrees
        const float deg2rad = 0.01745329f;  //Converts degrees to radians
        public float x, y, z, w;            //Position of the vector

        //Default constructor 
        public Vectors()
        {
            x = 0;
            y = 0;
            z = 0;
            w = 0;
        }

        /// <summary>
        /// Set the coordinates of the vector (2D)
        /// </summary>
        /// <param name="xValue">X position</param>
        /// <param name="yValue">Y position</param>
        public Vectors(float xValue, float yValue)
        {
            x = xValue;
            y = yValue;
            z = 0;
            w = 0;
        }

        /// <summary>
        /// Set the coordinates of the vector (3D)
        /// </summary>
        /// <param name="xValue">X position</param>
        /// <param name="yValue">Y position</param>
        /// <param name="zValue">Z position</param>
        public Vectors(float xValue, float yValue, float zValue)
        {
            x = xValue;
            y = yValue;
            z = zValue;
            w = 0;
        }

        /// <summary>
        /// Set the coordinates of the vector (4D)
        /// </summary>
        /// <param name="xValue">X position</param>
        /// <param name="yValue">Y position</param>
        /// <param name="zValue">Z position</param>
        /// <param name="wValue">W position</param>
        public Vectors(float xValue, float yValue, float zValue, float wValue)
        {
            x = xValue;
            y = yValue;
            z = zValue;
            w = wValue;
        }

        ////////////////////////////////(INPUT)////////////////////////////////////

        /// <summary>
        /// Set the coordinates of the vector (2D)
        /// </summary>
        /// <param name="xValue">X position</param>
        /// <param name="yValue">Y position</param>
        public void set(float xValue, float yValue)
        {
            x = xValue;
            y = yValue;
            z = 0;
            w = 0;
        }

        /// <summary>
        /// Set the coordinates of the vector (3D)
        /// </summary>
        /// <param name="xValue">X position</param>
        /// <param name="yValue">Y position</param>
        /// <param name="zValue">Z position</param>
        public void set(float xValue, float yValue, float zValue)
        {
            x = xValue;
            y = yValue;
            z = zValue;
            w = 0;
        }

        /// <summary>
        /// Set the coordinates of the vector (4D)
        /// </summary>
        /// <param name="xValue">X position</param>
        /// <param name="yValue">Y position</param>
        /// <param name="zValue">Z position</param>
        /// <param name="wValue">W position</param>
        public void set(float xValue, float yValue, float zValue, float wValue)
        {
            x = xValue;
            y = yValue;
            z = zValue;
            w = wValue;
        }

        /// <summary>
        /// Set the coordinates of the vector in polar form (2D)
        /// </summary>
        /// <param name="mag">Magnitude</param>
        /// <param name="head">Heading</param>
        public void setPolar(float mag, float head)
        {
            // cos(theta) = adjacent / hypoth solved for adjacent
            x = cos(head * deg2rad) * mag;
            // sin(theta) = opposite / hypoth solved for opposite
            y = sin(head * deg2rad) * mag;

            z = 0;
            w = 0;
        }

        /// <summary>
        /// Set coordinates of the vector given a magnitude, heading and pitch (3D)
        /// </summary>
        /// <param name="mag">Magnitude</param>
        /// <param name="head">Heading</param>
        /// <param name="pit">Pitch</param>
        public void setMagHeadPitch(float mag, float head, float pit)
        {
            // ||v|| * cos P * cos H
            x = mag * cos(pit * deg2rad) * cos(head * deg2rad);
            //finds y value based of formual:
            // ||v|| * cos P * sin H
            y = mag * cos(pit * deg2rad) * sin(head * deg2rad);
            //finds z value based of formual:
            // ||v|| * sin P
            z = mag * sin(pit * deg2rad);
            w = 0;
        }

        /// <summary>
        /// Returns magnitude of vector
        /// </summary>
        /// <returns></returns>
        public float getMagnitude()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Returns Pitch, the angle this vector makes with the x/y plane
        /// </summary>
        /// <returns></returns>
        public float getPitch()
        {
            //Checks for the zero vector
            if (x == 0 && y == 0 && z == 0)
                return 0;	//Returns 0 to avoid returning garbage

            //sin P = vz / ||v||
            float pitch = asin(z / getMagnitude());
            pitch = rad2deg * pitch;	//Convert the output to degrees

            return pitch;	//return value
        }

        /// <summary>
        /// Returns heading, the angle this vector makes with the positive x axis
        /// </summary>
        /// <returns></returns>
        public float getHeading()
        {
            //checks for the zero vector, checking only the x and y coodinates
            if (x == 0 && y == 0)
                return 0;	//returns 0 to avoid returning garbage

            //cos H = vx / sqrt( vx^2 + vy^2)
            float heading = acos(x / (float)Math.Sqrt(x * x + y * y));
            heading = rad2deg * heading;	//convert radians to degrees

            //Corrects for constriction of acos
            if (y < 0)
                heading = 360 - heading;

            return heading;
        }

        /// <summary>
        /// Returns alpha euler angle
        /// </summary>
        /// <returns></returns>
        public float getAlpha()
        {
            //checks for the zero vector
            if (x == 0 && y == 0 && z == 0)
                return 0;	//returns 0 to avoid returning garbage

            //cos alpha = vx / ||v||
            float alpha = acos(x / getMagnitude());
            alpha = rad2deg * alpha; //convert radians to degrees

            //Corrects for constriction of acos
            if (y < 0)
                alpha = 360 - alpha;

            return alpha;
        }

        /// <summary>
        /// Returns beta euler angle 
        /// </summary>
        /// <returns></returns>
        public float getBeta()
        {
            //checks for the zero vector
            if (x == 0 && y == 0 && z == 0)
                return 0;	//returns 0 to avoid returning garbage

            //returns beta angle based on the formula: cos beta = vy / ||v||
            float beta = acos(y / getMagnitude());
            beta = rad2deg * beta; //convert radians to degrees

            return beta;
        }

        /// <summary>
        /// Returns gamma euler angle 
        /// </summary>
        /// <returns></returns>
        public float getGamma()
        {
            //checks for the zero vector
            if (x == 0 && y == 0 && z == 0)
                return 0;	//returns 0 to avoid returning garbage

            //cos gamma = vz / ||V||
            float gamma = acos(z / getMagnitude());
            gamma = rad2deg * gamma; //Convert radians to degrees

            return gamma;
        }

        //returns x value for vector's endpoint
        public float getX()
        {
            return x;
        }

        //returns y value for vector's endpoint
        public float getY()
        {
            return y;
        }

        //returns z value for vector's endpoint
        public float getZ()
        {
            return z;
        }

        //returns z value for vector's endpoint
        public float getW()
        {
            return w;
        }

        //sets x value for vector's endpoint
        public void setX(float Value)
        {
            x = Value;
        }

        //sets y value for vector's endpoint
        public void setY(float Value)
        {
            y = Value;
        }

        //sets z value for vector's endpoint
        public void setZ(float Value)
        {
            z = Value;
        }

        //sets w value for vector's endpoint
        public void setW(float Value)
        {
            w = Value;
        }

        /// <summary>
        /// Vector Addition
        /// </summary>
        /// <param name="vect1"></param>
        /// <param name="vect2"></param>
        /// <returns></returns>
        public static Vectors operator +(Vectors vect1, Vectors vect2)
        {
            Vectors sum = new Vectors();	        //create a vector to store the sum of the two vectors
            sum.setX(vect1.getX() + vect2.getX());  //adds x values
            sum.setY(vect1.getY() + vect2.getY());  //adds y values
            sum.setZ(vect1.getZ() + vect2.getZ());  //adds z values
            return sum;	//return sum
        }

        /// <summary>
        /// vector Subtraction
        /// </summary>
        /// <param name="vect1"></param>
        /// <param name="vect2"></param>
        /// <returns></returns>
        public static Vectors operator -(Vectors vect1, Vectors vect2)
        {
            Vectors dif = new Vectors();            //create a vector to store the difference of the two vectors
            dif.setX(vect1.getX() - vect2.getX());	//subracts x values
            dif.setY(vect1.getY() - vect2.getY());	//subracts y values
            dif.setZ(vect1.getZ() - vect2.getZ());	//subracts z values
            return dif;	//return difference
        }

        /// <summary>
        /// Scalar vector multiplication
        /// </summary>
        /// <param name="vect1"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Vectors operator &(Vectors vect1, float amount)
        {
            Vectors product = new Vectors();        //create a vector to store the product 
            product.setX(amount * vect1.getX());	//calculate product of x values
            product.setY(amount * vect1.getY());	//calculate product of y values
            product.setZ(amount * vect1.getZ());	//calculate product of z values
            return product; //returns product
        }

        /// <summary>
        /// Normalizes a vector
        /// </summary>
        /// <param name="vect1"></param>
        /// <returns></returns>
        public static Vectors operator !(Vectors vect1)
        {
            Vectors normal = new Vectors(); //create a vector to store normalized vector
            //Divides vector by its magnitude
            float normalize;
            float mag = vect1.getMagnitude();
            if (mag != 0)
                normalize = 1 / mag;
            else
                normalize = 0;

            //Uses scalar multiplication to multiply the normalal with each value
            normal.setX(vect1.getX() * normalize);
            normal.setY(vect1.getY() * normalize);
            normal.setZ(vect1.getZ() * normalize);

            return normal;	//return the result
        }

        /// <summary>
        /// Returns the dot product of two vectors
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static float operator *(Vectors vect1, Vectors vect2)
        {
            float dot;	//create a variable to hold result of the dot product
            //calculate dot product using formula mentioned above
            dot = (vect1.getX() * vect2.getX()) +
                  (vect1.getY() * vect2.getY()) +
                  (vect1.getZ() * vect2.getZ());
            return dot;	//return dot product
        }

        /// <summary>
        /// Returns the angle between two vectors
        /// </summary>
        /// <param name="vect1"></param>
        /// <param name="vect2"></param>
        /// <returns></returns>
        public static float operator %(Vectors vect1, Vectors vect2)
        {
            float angle = vect1.acos((vect2 * vect1) / (vect1.getMagnitude() * vect1.getMagnitude()));
            return angle;
        }

        /// <summary>
        /// Finds the parallel projection of one vector onto another.
        /// </summary>
        /// <param name="vect1"></param>
        /// <param name="vect2"></param>
        /// <returns></returns>
        public static Vectors operator ^(Vectors vect1, Vectors vect2)
        {
            //Creates a second vector for storing the resuting parallel projection
            Vectors par = new Vectors();

            //Calculates the value of the dot product divided by the square of the 
            //magnitude of the caller vector.
            float amount = (vect2 * vect1) / (vect1.getMagnitude() * vect1.getMagnitude());

            //Multiplies amount to each vlaue in the vector
            par.setX(vect1.getX() * amount);
            par.setY(vect1.getY() * amount);
            par.setZ(vect1.getZ() * amount);

            return par; //return the result
        }

        /// <summary>
        /// Perpendicular projection of one vector onto another.
        /// </summary>
        /// <param name="vect1"></param>
        /// <param name="vect2"></param>
        /// <returns></returns>
        public static Vectors operator |(Vectors vect1, Vectors vect2)
        {
            //creates a vector to store the perpendicular projection and sets 
            //its value to the parallel projection of caller onto vector
            Vectors perp = vect1 ^ vect2;
            //subracts the perp vector from the caller vector
            perp = vect1 - perp;
            return perp;	//return perpindicular projection
        }


        /// <summary>
        /// Calculates the cross product.
        /// </summary>
        /// <param name="U"></param>
        /// <param name="V"></param>
        /// <returns></returns>
        public Vectors crossProduct(Vectors U, Vectors V)
        {
            //U X V = (uy * vz - vy * uz)x -(ux * vz - vx * uz)y +(ux * vy - vx * uy)z
            float cx, cy, cz;	//create variables to store resulting coordinates 
            cx = V.getY() * U.getZ() - U.getY() * V.getZ();	//x value
            cy = -(V.getX() * U.getZ() - U.getX() * V.getZ());	//y value
            cz = V.getX() * U.getY() - U.getX() * V.getY();	//z value

            return new Vectors(cx, cy, cz);	//return resulting vector
        }

        /// <summary>
        /// This calculates the closest point to a line.
        /// </summary>
        /// <param name="P">Object's location</param>
        /// <param name="Q">Point</param>
        /// <param name="D"></param>
        /// <returns></returns>
        public Vectors closestpointLine(Vectors P, Vectors Q, Vectors D)
        {
            Vectors PQ = Q - P;
            return (P + (D ^ PQ));	//return resulting vector
        }

        /// <summary>
        /// This finds the closest point on a plane given a starting point and three points representing a plane
        /// </summary>
        /// <param name="Q">Point</param>
        /// <param name="A">First Point</param>
        /// <param name="B">Second Point</param>
        /// <param name="C">Third Point</param>
        /// <returns></returns>
        public Vectors closestpointPlane(Vectors Q, Vectors A, Vectors B, Vectors C)
        {
            Vectors U = B - A;
            Vectors V = C - A;
            Vectors N = crossProduct(U, V);
            Vectors PQ = Q - A;
            return (Q - (N ^ PQ));	//return resulting vector
        }

        /// <summary>
        /// Returns dot product (4D)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public float dotProduct4D(Vectors v1, Vectors v2)
        {
            float dot;
            // Dot product = (Vx * Ux) + (Vy * Uy) + (Vz * Uz) + (Vw * Uw)
            dot = (v1.getX() * v2.getX()) + (v1.getY() * v2.getY()) +
                  (v1.getZ() * v2.getZ()) + (v1.getW() * v2.getW());
            return dot;	//returns dot product
        }

        /// <summary>
        /// This method translats a vertex by adding a vector quantity (trans)
        /// </summary>
        /// <param name="trans"></param>
        public void translation(Vectors trans)
        {
            Vectors result;	//Creates a vector to store the result
            result = trans + this;	//Adds the two vectors to translate the vertex
            setX(result.getX());	//set x value
            setY(result.getY());	//set y value
            setZ(result.getZ());	//set z value
        }

        /// <summary>
        /// This method scales a vertex through scalar multiplication.
        /// </summary>
        /// <param name="scale"></param>
        public void scale(Vectors scale)
        {
            //x = x * scaleX
            //y = y * scaleY
            //z = z * scaleZ
            Vectors result = new Vectors(getX() * scale.getX(),
                                         getY() * scale.getY(),
                                         getZ() * scale.getZ());

            //resulting vector is copied into the vector calling this method
            setX(result.getX());
            setY(result.getY());
            setZ(result.getZ());
        }

        /// <summary>
        /// This method scales a vertex about the origin using both tranlation and scaling
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="center"></param>
        public void scaleCenter(Vectors scale, Vectors center)
        {
            Vectors result = this - center;	//moves object to origin (translation)

            //scales object
            result.setX(result.getX() * scale.getX());
            result.setY(result.getY() * scale.getY());
            result.setZ(result.getZ() * scale.getZ());

            result = result + center;	//returns object to original location

            //resulting vector is copied into the vector calling this method
            setX(result.getX());
            setY(result.getY());
            setZ(result.getZ());
        }

        /// <summary>
        /// Rotate an object about the x axis.
        /// </summary>
        /// <param name="obj">Array of vectors representing an object</param>
        /// <param name="angle">Angle the object will be rotated by</param>
        public void rotateX(Vectors[] obj, float angle)
        {
            Vectors temp = new Vectors();
            Vectors[] rotMatrix = new Vectors[4];

            angle *= deg2rad;

            rotMatrix[0].set(1, 0, 0, 0);
            rotMatrix[1].set(0, cos(angle), -sin(angle), 0);
            rotMatrix[2].set(0, sin(angle), cos(angle), 0);
            rotMatrix[3].set(0, 0, 0, 1);

            for (int i = 0; i < obj.GetLength(0); i++)
            {
                temp.setX(temp.dotProduct4D(rotMatrix[0], obj[i]));
                temp.setY(temp.dotProduct4D(rotMatrix[1], obj[i]));
                temp.setZ(temp.dotProduct4D(rotMatrix[2], obj[i]));
                temp.setW(temp.dotProduct4D(rotMatrix[3], obj[i]));

                obj[i] = temp;
            }
        }

        /// <summary>
        /// Rotate an object about the Y axis.
        /// </summary>
        /// <param name="obj">Array of vectors representing an object</param>
        /// <param name="angle">Angle the object will be rotated by</param>
        public void rotateY(Vectors[] obj, float angle)
        {
            Vectors temp = new Vectors();
            Vectors[] rotMatrix = new Vectors[4];

            angle *= deg2rad;

            rotMatrix[0].set(cos(angle), 0, sin(angle), 0);
            rotMatrix[1].set(0, 1, 0, 0);
            rotMatrix[2].set(-sin(angle), 0, cos(angle), 0);
            rotMatrix[3].set(0, 0, 0, 1);

            for (int i = 0; i < obj.GetLength(0); i++)
            {
                temp.setX(temp.dotProduct4D(rotMatrix[0], obj[i]));
                temp.setY(temp.dotProduct4D(rotMatrix[1], obj[i]));
                temp.setZ(temp.dotProduct4D(rotMatrix[2], obj[i]));
                temp.setW(temp.dotProduct4D(rotMatrix[3], obj[i]));

                obj[i] = temp;
            }
        }

        /// <summary>
        /// Rotate an object about the Z axis.
        /// </summary>
        /// <param name="obj">Array of vectors representing an object</param>
        /// <param name="angle">Angle the object will be rotated by</param>
        public void rotateZ(Vectors[] obj, float angle)
        {
            Vectors temp = new Vectors();
            Vectors[] rotMatrix = new Vectors[4];

            angle *= deg2rad;

            rotMatrix[0].set(cos(angle), -sin(angle), 0, 0);
            rotMatrix[1].set(sin(angle), cos(angle), 0, 0);
            rotMatrix[2].set(0, 0, 1, 0);
            rotMatrix[3].set(0, 0, 0, 1);

            for (int i = 0; i < obj.GetLength(0); i++)
            {
                temp.setX(temp.dotProduct4D(rotMatrix[0], obj[i]));
                temp.setY(temp.dotProduct4D(rotMatrix[1], obj[i]));
                temp.setZ(temp.dotProduct4D(rotMatrix[2], obj[i]));
                temp.setW(temp.dotProduct4D(rotMatrix[3], obj[i]));

                obj[i] = temp;
            }
        }

        /// <summary>
        /// Checks to see if a pair of vectors are equal.
        /// </summary>
        /// <param name="vect1"></param>
        /// <param name="vect2"></param>
        /// <returns></returns>
        public static bool areEqual(Vectors vect1, Vectors vect2)
        {
            if (vect1.x == vect2.x &&
                vect1.y == vect2.y &&
                vect1.z == vect2.z &&
                vect1.w == vect2.w)
                return true;
            else
                return false;
        }

        public static float getDirection(float x, float y, float x2, float y2)
        {
            return getDirection(new Vectors(x, y), new Vectors(x2, y2));
        }

        public static float getDirection(Vectors v1, Vectors v2)
        {
            v2 = v2 - v1;


            //checks for the zero vector, checking only the x and y coodinates
            if (v2.x == 0 && v2.y == 0)
                return 0;	//returns 0 to avoid returning garbage

            //cos H = vx / sqrt( vx^2 + vy^2)
            float heading = (float)Math.Acos(v2.x / (float)Math.Sqrt(v2.x * v2.x + v2.y * v2.y));
            heading = rad2deg * heading;	//convert radians to degrees

            //Corrects for constriction of acos
            if (v2.y < 0)
                heading = 360 - heading;

            return heading;
        }

        ////////////////////////////////(Trig Functions)///////////////////////////////

        //These are basic trig functions that return floats
        float cos(float value)
        {
            return (float)Math.Cos(value);
        }

        float sin(float value)
        {
            return (float)Math.Sin(value);
        }

        float tan(float value)
        {
            return (float)Math.Tan(value);
        }

        //These are you inversed trig functions
        float acos(float value)
        {
            return (float)Math.Acos(value);
        }

        float asin(float value)
        {
            return (float)Math.Asin(value);
        }

        float atan(float value)
        {
            return (float)Math.Atan(value);
        }




    }
}