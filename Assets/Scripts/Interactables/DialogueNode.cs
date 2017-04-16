/* NAME:            Interactable.cs
 * AUTHOR:          Shinlynn Kuo, Yu-Che Cheng (Jeffrey), Hamza Awad, Emmilio Segovia
 * DESCRIPTION:     The node to construct a dialogue tree in an interactable class.
 * REQUIREMENTS:    None
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNode : MonoBehaviour
{
        //if not QandA (single message, or item)
        public string message = "no message";
        public DialogueNode NextNode;
        //if QandA
        public List<string> QandA; //[0] is Question, the rest are Answers
        public List<DialogueNode> children; //next child determined by QandA answer
        private string ItemName;
        private int ItemQuantity = 0; //default: flag that there is no item

        /// <summary>
        /// This constructs a single message Node.
        /// next_node is optional, its default is "null" meaning there are noot anymore
        /// Nodes. You may send null for readability if so desired.
        /// </summary>
        public DialogueNode(string new_message, DialogueNode next_node)
        {
            message = new_message;
            NextNode = next_node;
        
        }

        /// <summary>
        /// This constructs QandA nodes which take in a List<string>.
        /// Index [0] is the question and the rest are answers for the player to choose.
        /// Do not make these null otherwise dialogue will try to send a single message
        /// that does not exist. The lists' nullity are flags for the type of Node it is.
        /// </summary>
        public DialogueNode(List<string> q_and_a, List<DialogueNode> my_children)
        {
            QandA = q_and_a;
            children = my_children;
        }
        
        /// <summary>
        /// This constructs item-giving nodes.
        /// The item will be added to the Inventory when this dialogue's Next() is called.
        /// next_node is optional and defaults to null if the conversation ends here.
        /// These nodes do not send messages since adding an item to inventory automatically
        /// invokes a message to the player of what item(s) is/are obtained.
        /// </summary>
        public DialogueNode(string item_name, int item_quantity, DialogueNode next_node)
        {
            ItemName = item_name;
            ItemQuantity = item_quantity;
            NextNode = next_node;
        }

        /// <summary>
        /// This is invokes the DialogueNode to perform its action.
        /// It decides whether to print a single message, give an item to
        /// inventory, or display a QandA message based on variables set by the constructor.
        /// </summary>
        public void Run(Interactable caller)
        {
        if (ItemQuantity == 0 && QandA == null) {
            UIManager.Instance.RunDialogue(caller, message);
        }
        else if (ItemQuantity > 0) {
            Interactable.AddItem(caller, ItemName, ItemQuantity);
            GameManager.Instance.SetReceived(caller.GetType().ToString());
        }
        else {
            UIManager.Instance.RunDialogue(caller, QandA);
        }
        }

        /// <summary>
        /// Return the single child of this Node (default).
        /// </summary>
        public DialogueNode GetNext()
        {
            return NextNode;
        }

        /// <summary>
        /// Overrides the default Next() function for when a particular
        /// child needs to be called based on the player's answer to a QandA.
        /// </summary>
        public DialogueNode GetNext(int child_answer)
        {
            if (child_answer >= 0 && child_answer <= children.Count) {
                return children[child_answer];
            }
            //else
            Debug.LogError("Out of range response to QandA.");
            return null;
        }

        public bool HasNext()
        {
            if (!ReferenceEquals(NextNode, null) || (!ReferenceEquals(children, null) && children.Count > 0))
                return true;
            //else
            return false;
        }
}
