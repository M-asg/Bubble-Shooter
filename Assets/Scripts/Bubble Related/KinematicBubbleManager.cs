using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KinematicBubbleManager : MonoBehaviour
{
    //variable
    [SerializeField] float waitBeforeMove = 1f;
    [SerializeField] float waitBeforeCheckDrop = 0.9f;

    [SerializeField] float MoveSpeed = 0.01f;
    [SerializeField] int maxVerticalBubbles = 8;

    //configs
    List<KinematicBubble> kinematicBubbles = new List<KinematicBubble>();
    List<float> lastSelectedBubbleYPos = new List<float>();
    List<KinematicBubble> checkforDropList = new List<KinematicBubble>();


    //state
    bool scrollReachRoof = false;
    bool rdyToShoot = false;

    int lastRemainngBubbleCount = 0;
    float currentAmount = 10;

    bool checkForScroll = false;
    float timePassed = 0;

    bool checkForDrop;
    float timePassedForstartCheckingForDrop = 0;
    [HideInInspector]
    public bool gameStart = false;


    private void Awake()
    {
        gameStart = true;
        GameObject[] PlaceHolders = GameObject.FindGameObjectsWithTag("PlaceHolder");
        foreach (var placeholder in PlaceHolders)
        {
            DestroyImmediate(placeholder);
        }
    }


    private void Start()
    {
        lastRemainngBubbleCount = kinematicBubbles.Count;
    }

    private void FixedUpdate()
    {
        if (checkForScroll)
        {
            timePassed += Time.deltaTime;
            if(timePassed > waitBeforeMove)
            {
                //change amount
                changeAmount();
                checkForScroll = false;
                timePassed = 0;
                CheckForYPosAndScroll();
            }
        }
        if (checkForDrop)
        {
            timePassedForstartCheckingForDrop += Time.deltaTime;
            if(timePassedForstartCheckingForDrop > waitBeforeCheckDrop)
            {
                AttempToDrop();
                checkForDrop = false;
            }
        }
        
        
    }

    private void changeAmount()
    {
        if(lastRemainngBubbleCount > kinematicBubbles.Count)
        {
            currentAmount += 10;
            lastRemainngBubbleCount = kinematicBubbles.Count;
        }
        else
        {
            if(currentAmount > 20)
            {
                currentAmount -= 20;
            }
        }
    }

    private void AttempToDrop()
    {
        List<KinematicBubble> uniqDropBubble = checkforDropList.Distinct().ToList();
        List<float> yPos = new List<float>();


        foreach (KinematicBubble checkforDropBubble in uniqDropBubble)
        {
            yPos.Add(checkforDropBubble.transform.position.y);
        }
        float maxYPos = yPos.Max();
        float minYPos = yPos.Min();
        

        while ((int)maxYPos >= (int)minYPos)
        {
            foreach (KinematicBubble bubble in uniqDropBubble)
            {
                if ((int)bubble.transform.position.y + 0.25f == (int)maxYPos + 0.25f)
                {
                    CheckForDrop(bubble);
                }
            }
            maxYPos--;
        }
        timePassedForstartCheckingForDrop = 0;
        checkforDropList.Clear();
    }
    public void AddToKinematicList(KinematicBubble kinematicBubble)
    {
        // maybe change to interface
        kinematicBubbles.Add(kinematicBubble);
    }
    public void RemoveFromKinematicList(KinematicBubble kinematicBubble)
    {
        kinematicBubbles.Remove(kinematicBubble);
    }

    private void CheckForDrop(KinematicBubble selectedBubble)
    {
        bool thereIsNotSelectedBubbleOnMyLine = true;
        for (int i = 0; i < lastSelectedBubbleYPos.Count; i++)
        {
            if (selectedBubble.transform.position.y == lastSelectedBubbleYPos[i])
            {
                thereIsNotSelectedBubbleOnMyLine = false;
            }
        }
        if (thereIsNotSelectedBubbleOnMyLine)
        {
            // find all sideNeighbours
            if (!selectedBubble.CheckForAnchor())
            {
                List<KinematicBubble> sideBubbles = new List<KinematicBubble>();
                lastSelectedBubbleYPos.Add(selectedBubble.transform.position.y);
                selectedBubble.FindNeighborsAndAddToList(sideBubbles, selectedBubble.transform.position);

                //foreach (KinematicBubble bubble in sideBubbles)
                //{
                //    Debug.Log(selectedBubble.name + " find = " + bubble.name + " his Side Neighbour");
                //}

                //check for anchor in that line

                bool orAllSideAnchors = false;
                foreach (KinematicBubble bubble in sideBubbles)
                {
                    orAllSideAnchors = orAllSideAnchors || bubble.CheckForAnchor();
                }

                if (orAllSideAnchors == false)
                {
                    foreach (KinematicBubble bubble in sideBubbles)
                    {
                        bubble.SetAnchor(false);
                    }
                    bool continueWhile = true;
                    while (continueWhile)
                    {
                        List<KinematicBubble> buttomBubbles = new List<KinematicBubble>();
                        foreach (KinematicBubble bubble in sideBubbles)
                        {
                            bubble.FindButtomsAndAddToList(buttomBubbles);
                        }
                        List<KinematicBubble> uniqButtomBubbles = buttomBubbles.Distinct().ToList();



                        List<KinematicBubble> buttomSide = new List<KinematicBubble>();
                        foreach (KinematicBubble bubble in uniqButtomBubbles)
                        {
                            bubble.FindNeighborsAndAddToList(buttomSide, bubble.transform.position);
                        }
                        List<KinematicBubble> uniqButtomSides = buttomSide.Distinct().ToList();


                        //check if there is no layer
                        if (uniqButtomSides.Count == 0)
                        {
                            // Debug.Log(selectedBubble.name+" Called Drop For Every one");
                            selectedBubble.Drop(selectedBubble.transform.position);
                            lastSelectedBubbleYPos.Clear();
                            continueWhile = false;
                        }
                        else
                        {
                            bool orAllAnchorsButtoms = false;
                            foreach (KinematicBubble bubble1 in uniqButtomSides)
                            {
                              //  Debug.Log(selectedBubble.name + " find = " + bubble1.name + " his ButtonSide Neighbour");
                                orAllAnchorsButtoms = orAllAnchorsButtoms || bubble1.CheckForAnchor();
                            }

                            if (orAllAnchorsButtoms == false)
                            {
                                // redo this while loop
                                foreach (KinematicBubble bubble in uniqButtomSides)
                                {
                                    bubble.SetAnchor(false);
                                }
                                sideBubbles = uniqButtomSides;
                            }
                            else
                            {
                                // dont continue and return
                                foreach (KinematicBubble bubble in uniqButtomSides)
                                {
                                    bubble.SetAnchor(true);
                                }
                                continueWhile = false;
                            }
                        }
                    }
                    //clear
                    lastSelectedBubbleYPos.Clear();
                }
                else
                {
                    foreach(KinematicBubble bubble in sideBubbles)
                    {
                        bubble.SetAnchor(true);
                    }
                    lastSelectedBubbleYPos.Clear();
                    //clear
                }
            }
        }
    }

    private void CheckForYPosAndScroll()
    {

        List<float> yDistances = new List<float>();
        foreach (KinematicBubble bubble in kinematicBubbles)
        {
            yDistances.Add(CalculateYDistanceFromOrigin(Vector2.up * (10 - (maxVerticalBubbles+1) ), bubble.transform.position));
        }
        float minYDistance;
        float maxYDistance;
        if (yDistances.Count > 0)
        {
             minYDistance = yDistances.Min();
             maxYDistance = yDistances.Max();

            // Debug.Log("minYDistance = " + minYDistance);
            // Debug.Log("maxYDistance = " + maxYDistance);

            if (maxYDistance > maxVerticalBubbles - 1 + minYDistance)
            {
                float extended = 0;
                if (scrollReachRoof)
                {
                    extended = 0.25f;
                }
                Vector2 finalPosition = new Vector2(0, transform.position.y + extended - minYDistance);
                StartCoroutine(MoveTorward(finalPosition));
                scrollReachRoof = false;
            }
            else
            {
                if (!scrollReachRoof)
                {
                    scrollReachRoof = true;
                    Vector2 finalPosition = new Vector2(0, (transform.position.y - 0.25f) - (maxYDistance - (maxVerticalBubbles - 1)));
                    StartCoroutine(MoveTorward(finalPosition));
                }
            }
        }
        

    }

    private IEnumerator MoveTorward(Vector2 finalPosition)
    {
        rdyToShoot = false;
        while (Vector2.Distance(transform.position , finalPosition) >= 0.001f)
        {
            //Debug.Log("in while");
            transform.position = Vector2.MoveTowards(transform.position , finalPosition, MoveSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        rdyToShoot = true;
    }

    public void SetTimerAndCheckForScroll()
    {
        timePassed = 0;
        checkForScroll = true;
    }

    public List<string> GetBubbleTypesThatLeft()
    {
        List<string> tags = new List<string>();
        foreach(KinematicBubble bubble in kinematicBubbles)
        {
            switch (bubble.tag)
            {
                case "Red Bubble": tags.Add("Red Bubble"); break;
                case "Yellow Bubble": tags.Add("Yellow Bubble"); break;
                case "Purple Bubble": tags.Add("Purple Bubble"); break;
                case "Green Bubble": tags.Add("Green Bubble"); break;
            }   
        }
        List<string> uniqTags = tags.Distinct().ToList();
        return uniqTags;
    }
    private float CalculateYDistanceFromOrigin(Vector2 origin,Vector2 position)
    {
        Vector2 chord = position - origin;
        float angle = Vector2.Angle(chord , Vector2.right);
        float yDistance = Mathf.Sin(angle * Mathf.PI / 180) * chord.magnitude;
        if (position.y - origin.y >= 0)
        {
            return (int)(yDistance);
        }
        else
        {
            return -Mathf.Round(yDistance);
        }
    }

    public void SetTimerAndCheckForDropAndAddToList(KinematicBubble checkForDropBubble)
    {
        checkforDropList.Add(checkForDropBubble);
        timePassedForstartCheckingForDrop = 0;
        checkForDrop = true;
    }

    public void SetTimerAndCheckForDropAndRemoveFromList(KinematicBubble checkForDropBubble)
    {
        if (checkforDropList.Contains(checkForDropBubble))
        {
            checkforDropList.Remove(checkForDropBubble);
            timePassedForstartCheckingForDrop = 0;
            checkForDrop = true;
        }
    }

    public int GetRemainingBubbleCount()
    {
        return kinematicBubbles.Count;
    }

    public void SetTimerForDropToZero()
    {
         timePassedForstartCheckingForDrop = 0;
    }

    public bool RdyToShoot()
    {
        return !checkForDrop && !checkForScroll && rdyToShoot;
    }


    public float GetAmount()
    {
        return currentAmount;
    }
}
