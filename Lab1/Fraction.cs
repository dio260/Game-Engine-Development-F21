using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    
        public struct Fraction
        {
            private int numerator, denominator;

            public int Numerator
            {
                get { return numerator; }
                set { numerator = value; Simplify(); }
            }

            public int Denominator
            {
                get { return denominator; }
                set { denominator = value; Simplify(); }
            }
            public Fraction(int n = 0 , int d = 1)
            {
                numerator = n;
                if (d == 0)
                    d = 1;
                denominator = d;
                Simplify();
            }

            public override string ToString()
            {
                return numerator + "/" + denominator;
            }

            private void Simplify()
            {
                if (denominator < 0)
                {
                    denominator *= -1;
                    numerator *= -1;
                }
                int gcd = GCD(numerator, denominator);
                numerator /= gcd;
                denominator /= gcd;
            }

            public static Fraction operator +(Fraction lhs, Fraction rhs)
            {
                Fraction addedStruct = 
                    new Fraction(
                        (lhs.numerator * rhs.denominator) + (lhs.denominator * rhs.numerator),
                        (lhs.denominator * rhs.denominator) );
                return addedStruct;
            }

            public static Fraction operator -(Fraction lhs, Fraction rhs)
            {
                Fraction subtractedStruct =
                    new Fraction(
                        (lhs.numerator * rhs.denominator) - (lhs.denominator * rhs.numerator),
                        (lhs.denominator * rhs.denominator));
                return subtractedStruct;
            }

            public static Fraction operator *(Fraction lhs, Fraction rhs)
            {
                return new Fraction(lhs.numerator * rhs.numerator, lhs.denominator * rhs.denominator);
            }

            public static Fraction operator /(Fraction lhs, Fraction rhs)
            {
                return new Fraction(lhs.numerator * rhs.denominator, lhs.denominator * rhs.numerator);
            }

            public static int GCD(int a, int b)
            {
                int gcd;
            if (a == 0)
            {
                gcd = b;
            }
            else if (b == 0)
            {
                gcd = a;
            }
            else
            {
                int quotient = a / b;
                int remainder = a % b;
                gcd = GCD(b, remainder);
            }

                return gcd;
            }

        }

       
}
