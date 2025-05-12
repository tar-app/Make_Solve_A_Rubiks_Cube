using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateBigCube : MonoBehaviour
{
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    Vector3 previousMousePosition;
    Vector3 mouseDelta;

    public GameObject target;
    float speed = 200f;

    void Update()
    {
        Swipe();
        Drag();
    }

    void Drag()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            mouseDelta = (Vector3)Mouse.current.position.ReadValue() - previousMousePosition;
            mouseDelta *= 0.1f;
            transform.rotation = Quaternion.Euler(mouseDelta.z, -mouseDelta.x, -mouseDelta.y) * transform.rotation;
        }
        else
        {
            if (transform.rotation != target.transform.rotation)
            {
                var step = speed * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);
            }
        }

        previousMousePosition = Mouse.current.position.ReadValue();
    }

    void Swipe()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            firstPressPos = Mouse.current.position.ReadValue();
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            secondPressPos = Mouse.current.position.ReadValue();
            currentSwipe = secondPressPos - firstPressPos;
            currentSwipe.Normalize();


            if (LeftSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 90, 0, Space.World);
            }
            else if (RightSwipe(currentSwipe))
            {
                target.transform.Rotate(0, -90, 0, Space.World);
            }
            else if (UpSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, 90, Space.World);
            }
            else if (DownSwipe(currentSwipe))
            {
                target.transform.Rotate(0, 0, -90, Space.World);
            }
            else if (UpLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(90, 0, 0, Space.World);
            }
            else if (UpRightSwipe(currentSwipe))
            {
                target.transform.Rotate(-90, 0, 0, Space.World);
            }
            else if (DownLeftSwipe(currentSwipe))
            {
                target.transform.Rotate(90, 0, 0, Space.World);
            }
            else if (DownRightSwipe(currentSwipe))
            {
                target.transform.Rotate(-90, 0, 0, Space.World);
            }
        }
    }

    bool LeftSwipe(Vector2 swipe)
    {
        return swipe.x < 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }

    bool RightSwipe(Vector2 swipe)
    {
        return swipe.x > 0 && swipe.y > -0.5f && swipe.y < 0.5f;
    }
    bool UpSwipe(Vector2 swipe)
    {
        return swipe.y < 0 && swipe.x > -0.5f && swipe.x < 0.5f;
    }
    bool DownSwipe(Vector2 swipe)
    {
        return swipe.y > 0 && swipe.x > -0.5f && swipe.x < 0.5f;
    }
    bool UpLeftSwipe(Vector2 swipe)
    {
        return swipe.x < -0.5f && swipe.y < -0.5f;
    }
    bool UpRightSwipe(Vector2 swipe)
    {
        return swipe.x > 0.5f && swipe.y < -0.5f;
    }
    bool DownLeftSwipe(Vector2 swipe)
    {
        return swipe.x < -0.5f && swipe.y > 0.5f;
    }
    bool DownRightSwipe(Vector2 swipe)
    {
        return swipe.x > 0.5f && swipe.y > 0.5f;
    }
}
