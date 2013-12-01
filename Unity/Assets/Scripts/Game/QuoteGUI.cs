using UnityEngine;
using System.Collections;

public class QuoteGUI : MonoBehaviour
{
    public Balloon balloon;

    public TextMesh txtQuote;
    public TextMesh txtAuthor;

    public Camera camGUI;

    private float lastScreenWidth = -1f;

    private float TOTAL_WAIT_TIME = 30f;

    private void Start()
    {
        txtQuote.color = txtQuote.color.GetTransparent();
        txtAuthor.color = txtAuthor.color.GetTransparent();

        StartCoroutine(WaitForFirstQuote());
    }

    private void Update()
    {
        if (lastScreenWidth != Screen.width)
            ResetCam();
    }

    private IEnumerator WaitForFirstQuote()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                balloon.Lift();
                
                StartCoroutine(ShowQuote());
                break;
            }

            yield return null;
        }
    }

    private IEnumerator ShowQuote()
    {
        Color clrQuote;
        Color clrQuoteEnd;
        Color clrAuthor;
        Color clrAuthorEnd;
        float timer;
        float tweenTime;

        while (true)
        {
            QuoteData q;

            // this is all hacky. forgive me.
            while (true)
            {
                q = Quotes.GetQuote();

                if (q.quote.Length < 294)
                    break;

                yield return null;
            }

            string quote = q.quote;
            string author = string.Format("- {0}", string.IsNullOrEmpty(q.author) ? "anonymous" : q.author);

            quote = quote.Replace("<br>", "");
            quote = quote.WordWrap(49, 6);

            // TODO: somestimes author is empty

            txtQuote.text = quote;
            txtAuthor.text = author;

            // fade in
            clrQuote = txtQuote.color;
            clrQuoteEnd = txtQuote.color.GetOpaque();

            clrAuthor = txtAuthor.color;
            clrAuthorEnd = txtAuthor.color.GetOpaque();

            tweenTime = 0;

            while (tweenTime < 1f)
            {
                tweenTime += Time.deltaTime;

                txtQuote.text = quote.Substring(0, Mathf.Min(quote.Length, Mathf.FloorToInt(quote.Length * (tweenTime / 1f))));

                txtQuote.color = Color.Lerp(clrQuote, clrQuoteEnd, tweenTime);
                txtAuthor.color = Color.Lerp(clrAuthor, clrAuthorEnd, tweenTime);

                yield return null;
            }

            txtQuote.color = txtQuote.color.GetOpaque();
            txtAuthor.color = txtAuthor.color.GetOpaque();

            ////////////////////////////////

            timer = TOTAL_WAIT_TIME;

            while (true)
            {
                timer -= Time.deltaTime;

                if (timer < 0)
                    break;

                if (Input.GetMouseButtonDown(0))
                {
                    balloon.Lift();
                    break;
                }

                yield return null;
            }

            ////////////////////////////////

            // fade out
            clrQuote = txtQuote.color;
            clrQuoteEnd = txtQuote.color.GetTransparent();

            clrAuthor = txtAuthor.color;
            clrAuthorEnd = txtAuthor.color.GetTransparent();

            tweenTime = 0;

            while (tweenTime < 1f)
            {
                tweenTime += Time.deltaTime;

                txtQuote.color = Color.Lerp(clrQuote, clrQuoteEnd, tweenTime);
                txtAuthor.color = Color.Lerp(clrAuthor, clrAuthorEnd, tweenTime);

                yield return null;
            }

            txtQuote.color = clrQuoteEnd;
            txtAuthor.color = clrAuthorEnd;

            if (timer <= 0f)
            {
                balloon.Descend();

                while (true)
                {
                    timer += Time.deltaTime;

                    if (timer > TOTAL_WAIT_TIME)
                    {
                        timer -= TOTAL_WAIT_TIME;

                        balloon.Descend();
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        balloon.Lift();
                        break;
                    }

                    yield return null;
                }
            }
        }
    }

    private void ResetCam()
    {
        Rect guiRect = camGUI.rect;

        guiRect.width = 1f - (96f / Screen.width);
        guiRect.x = 96f / Screen.width;

        camGUI.rect = guiRect;

        lastScreenWidth = Screen.width;
    }
}
