

#ifndef __TLIST_H
#define __TLIST_H

#include <stddef.h>

/**********************************************************
CLASS
	TList

DESCRIPTION
	The container for Node's of the same time (class T).

EXAMPLES
     Going through all members of the list:
     <pre>
          for( Test *p = list.GetFirst();p;p = p->Next())
               ...
     </pre>

     The following is an example where the current node might be deleted, or moved while cycling through the list.  This prevents 
     problems.

     <pre>
          Test *pnext,*p;
          for(p = list.GetFirst();p;p = pnext)
          {
               pnext = p->Next();
               if (p->Finished())
                    delete p;         // Automaticly removes it from the list.
          }
     </pre>

     Deleting all members of a list:

     <pre>
          Test *p;
          while( (p = list.GetFirst()) )
               delete p;
     </pre>

AUTHOR
	Will Ware

***********************************************************/
template < class T > class TList
{
public:

/**********************************************************
CLASS 
	Node

DESCRIPTION
	Node for the TList.  This may be used as a base class or
	can be placed as a standalone class.   The only requirement is 
	that either the constructor Node( T *source ) or the T.SetData() be called
	before using.

EXAMPLES
     Including the node as a base class.
     <pre>
          class Test: public TList< Test >::Node
          {
               ...
          public:
               Test()        { SetData( this ); }
          };
     </pre>

     Including one or more nodes in the class.
     <pre>
          class Test
          {
               TList< Test >::Node       m_Node[2];
          public:
               Test()        { m_Node1.SetData(this); m_Node2.SetData(this); }

               TList< Test >::Node &    GetNode( int num )        { return m_Node[num]; }

          };
     </pre>

     In the case above here is how to add one of the nodes into a list.
     <pre>
          TList< Test > list;
          Test *p = new Test();

          list.AddHead( p->GetNode( 1 ) );
     </pre>

AUTHOR
	Will Ware

***********************************************************/
	class Node
	{
		friend TList<T>;

        //-----------------------------------------------------
        //  [] m_Next
        //
		Node		*m_Next;

        //-----------------------------------------------------
        //  [] m_Prev
        //
		Node		*m_Prev;

        //-----------------------------------------------------
        //  [] m_Data
        //
		T					*m_Data;

        //-----------------------------------------------------
        //  [use] removeQuick
        //
        //  [in]  void
        //
        //  [out] void
        //
        void    removeQuick( void )                 { if (m_Next) { m_Next->m_Prev = m_Prev;m_Prev->m_Next = m_Next; }}
	public:

				//-----------------------------------------------------
				//  [use] Node
				//		Creates a node that is out of the list.  Must call SetData() with a pointer to the structure that
				//		it is in before use.
				//
				Node( void )                  { m_Next = m_Prev = NULL;m_Data = NULL; }
				//-----------------------------------------------------
				//  [use] Node
				//		Creates a node that is not in the list.  Sets the data to the location header.
				//
				//  [in]  source
				//		Pointer to the 'this' pointer.
				//
				//		Example:	class CTest: Node<CTest> 
				//					{
				//						CTest()		{ SetData(this); }
				//					};
				//
				Node  ( T *source )           { m_Next = m_Prev = NULL;m_Data = source; }
				//-----------------------------------------------------
				//  [use] ~Node
				//		If the node is in a list it removes it before destruction.
				//
		~Node(void)  { 
			Remove(); 
		}

		//-----------------------------------------------------
		//  [use] Remove
		//		If the node is in a list it removes the node from the list.
		//

		void Remove(void)  {
			if (m_Next)  { 
				m_Next->m_Prev = m_Prev;
				m_Prev->m_Next = m_Next;
				m_Next = NULL;
			}
		}
		//-----------------------------------------------------
		//  [use] SetData
		//		Sets the data pointer of this list.  This pointer gets returned from calls such as Next() and Prev().
		//
		//  [in]  data
		//		Should be the 'this' pointer to the structure.
		//
		void    SetData         ( T *data )         { m_Data = data; }
		//-----------------------------------------------------
		//  [use] GetData
		//		Returns the 'this' pointer that should have been set with SetData().  If NULL then this Node is uninitalized.
		//
		//  [out] T*
		//		The 'this' ponter set by the SetData() call.
		//
		T *     GetData         ( void ) const      { return m_Data; }
		//-----------------------------------------------------
		//  [use] IsInList
		//		Returns 'true' if the node is currently in a list.
		//
		//  [out] bool
		//
		bool    IsInList        ( void ) const      { return (m_Next != NULL); }
		//-----------------------------------------------------
		//  [use] IsFirst
		//		Returns 'true' if the node is the first one in the list.
		//
		//  [out] bool
		//
		bool    IsFirst         ( void ) const      { return (m_Next ? (m_Prev->m_Prev == NULL):false); }
		//-----------------------------------------------------
		//  [use] IsLast
		//		Returns 'true' if it is the last node in the list.  Returns false if it is not in a list.
		//
		//  [out] bool
		//
		bool    IsLast          ( void ) const      { return (m_Next ? (m_Next->m_Next == NULL):false); }           

		//-----------------------------------------------------
		//  [use] Next
		//		Returns the next 'DATA' pointer in the list.  Note this is not the node itself but the 
		//		start of the structure.
		//
		//  [out] T*
		//		The current 'DATA' pointer.
		//
		T * Next                ( void ) const      { return (m_Next->m_Next ? m_Next->m_Data:NULL ); }

		//-----------------------------------------------------
		//  [use] Prev
		//		Returns the previous 'DATA' pointer in the list.  Note this is not the node itself but the
		//		a pointer to the base structure.
		//
		//  [out] T*
		//		The current 'DATA' pointer.
		//
		T * Prev                ( void ) const      { return (m_Prev->m_Prev ? m_Prev->m_Data:NULL ); }

		//-----------------------------------------------------
		//  [use] GetNextNode
		//		Returns the next node in the list.
		//
		//  [out] Node*
		//		next node or a NULL if it is at the end of the list.
		//
		Node * GetNextNode ( void ) const  { return (m_Next->m_Next ? m_Next:NULL ); }

		//-----------------------------------------------------
		//  [use] GetPrevNode
		//		Returns the previous node in the list.
		//
		//  [out] Node*
		//		previous node or a NULL if it is at the end of the list.
		//
		Node * GetPrevNode ( void ) const  { return (m_Prev->m_Prev ? m_Prev:NULL ); }
			
		//-----------------------------------------------------
		//  [use] GetList
		//		Returns a pointer list if it is in a list.
		//
		//  [out] TList<T>*
		//		The list pointer or NULL if the node is not currently in a list.
		//
		TList<T> *GetList( void ) const
		{
			Node	*ln = this;
			if (m_Next)
			{
				while( ln->m_Next )
					ln = ln->m_Next;
				return (TList<T> *)ln;
			}
			return NULL;
		}
		//-----------------------------------------------------
		//  [use] GetFirst
		//		Returns the first node in a list.  If it is not in a list then it returns this item.
		//
		//  [out] T*
		//
		T *  GetFirst           ( void ) const
		{
			Node	*ln = this;
			if (m_Next)
			{
				while( ln->m_Prev->m_Prev )
					ln = ln->m_Prev;
			}
			return ln->m_Data;
		}

		//-----------------------------------------------------
		//  [use] GetLast
		//		Returns the last node in the list.   If it is not in a list then it returns this item.
		//
		//  [out] T*
		//
		T *  GetLast        ( void ) const
		{
			Node	*ln = this;
			if (m_Next)
			{
				while( ln->m_Next->m_Next )
					ln = ln->m_Next;
			}
			return ln->m_Data;
		}
		//-----------------------------------------------------
		//  [use] InsertAfter
		//		Inserts this node AFTER 'ln'.  'ln' is assumed to be in a list.
		//
		//  [in/out]  ln
		//		Node& ln.   WARNING: Must be in a list or errors will develope!
		//
		void    InsertAfter     ( Node &ln )
		{
			removeQuick();
			m_Next = ln.m_Next;
			m_Prev = &ln;
			ln.m_Next->m_Prev = this;
			ln.m_Next = this;
		}

		//-----------------------------------------------------
		//  [use] InsertBefore
		//		Inserts this node BEFORE 'ln'.  'ln' is assumed to be in a list.
		//
		//  [in/out]  ln
		//		Node& ln.   WARNING: Must be in a list or errors will develope!
		//
		void    InsertBefore    ( Node &ln )
		{
			removeQuick();
			m_Next = &ln;
			m_Prev = ln.m_Prev;
			ln.m_Prev->m_Next = this;
			ln.m_Prev = this;
		}
};


	friend Node;
private:

    //-----------------------------------------------------
    //  [] m_Head
    //
	Node		*m_Head;

    //-----------------------------------------------------
    //  [] m_Tail
    //
	Node		*m_Tail;

    //-----------------------------------------------------
    //  [] m_TailPred
    //
	Node		*m_TailPred;

public:

	//-----------------------------------------------------
	//  [use] TList
	//		Initalizes the list to an empty list.

	TList()  { 
		m_Head = (Node *)&m_Tail; 
		m_Tail = NULL; 
		m_TailPred = (Node *)&m_Head; 
	}

	//-----------------------------------------------------
	//  [use] ~TList
	//		Removes all the nodes in the list before destroying the list.

	~TList()  { 
		RemoveAll(); 
	}

    //-----------------------------------------------------
    //  [use] RemoveAll
    //		Removes all the nodes from the list.
	//

    void RemoveAll (void)  { 
		 while(m_Head->m_Next)  {
			 m_Head->Remove(); 
		 }
	 }

    //-----------------------------------------------------
    //  [use] IsEmpty
    //		Return TRUE if the list has no nodes in it.
    //
    //  [out] bool
    //		TRUE if the list is empty.
	//
    bool    IsEmpty         ( void ) const  { return (m_Head->m_Next == NULL); }


    //-----------------------------------------------------
    //  [use] GetFirst
    //		Returns the data of the first node.
    //
    //  [out] T*
    //		Data of the first node or NULL if the list is empty.
	//
    T *     GetFirst        ( void ) const  { return (m_Head->m_Next == NULL ? NULL:m_Head->m_Data); }

    //-----------------------------------------------------
    //  [use] GetLast
    //		Returns the data of the last node.
    //
    //  [out] T*
    //		Data of the last node or NULL if the list is empty.
	//
    T *     GetLast         ( void ) const  { return (m_TailPred->m_Prev == NULL ? NULL:m_TailPred->m_Data ); }


    //-----------------------------------------------------
    //  [use] GetFirstNode
    //		Return the first Node in the list
    //
    //  [out] Node*
    //
    Node * GetFirstNode    ( void ) const	{ return (m_Head->m_Next == NULL ? NULL:m_Head); }

    //-----------------------------------------------------
    //  [use] GetLastNode
    //		Returns the last Node in the list.
    //
    //  [out] Node*
    //
    Node * GetLastNode     ( void ) const	{ return (m_TailPred->m_Prev == NULL ? NULL: m_TailPred); }


    //-----------------------------------------------------
    //  [use] Count
    //		
    //
    //  [out] int
    //		Number of nodes currently in the list.
	//
    int     Count           ( void ) const
	{
		Node	*ln = m_Head;
		int				i = 0;
		for(;ln->m_Next;ln = ln->m_Next,i++)
			;
		return i;
	}

    //-----------------------------------------------------
    //  [use] AddHead
	//		Adds a node to the start of the list.
    //
    //  [in/out]  ln
    //
    void    AddHead         ( Node &ln )   
	{ 
		ln.Remove(); 
		ln.m_Prev = (Node *)&m_Head;
		ln.m_Next = m_Head;
		ln.m_Next->m_Prev = m_Head = &ln; 
	}

    //-----------------------------------------------------
    //  [use] AddTail
    //		Adds a node to the end of the list.
	//
    //  [in/out]  ln
    //
    void    AddTail         ( Node &ln )   
	{ 
		ln.Remove(); 
		ln.m_Next = (Node *)&m_Tail;
		ln.m_Prev = m_TailPred;
		ln.m_Prev->m_Next = m_TailPred = &ln; 
	}

    //-----------------------------------------------------
    //  [use] RemoveHead
    //		Removes the first node (if there is one) from the list and returns it.
    //
    //  [out] T*
    //		The first node's data of the list or NULL if the list is empty.
	//
    T *     RemoveHead      ( void )
	{
		Node *ln = m_Head;
		if (ln->m_Next)
		{
			ln->Remove();
			return ln->m_Data;
		}
		return NULL;
	}

    //-----------------------------------------------------
    //  [use] RemoveTail
    //		Removes the last node (if there is one) from the list and returns it.
    //
    //  [out] T*
	//		The last node's data of the list or NULL if the list is empty.
    //
    T *     RemoveTail      ( void )
	{
		Node *ln = m_TailPred;
		if (ln->m_Prev)
		{
			ln->Remove();
			return ln->m_Data;
		}
		return NULL;
	}

    //-----------------------------------------------------
    //  [use] InsertSorted
    //		Inserts a node into the list in a sorted manner.  Assumes the list is already sorted.
	//
    //  [in/out]  ln
	//		Node to be inserted.
    //
    //  [in]  gefunc
    //		Sort function for this node.  Must return TRUE if (leftnode >= rightnode) 
    //
    void    InsertSorted    ( Node &ln, bool( *gefunc)(T &leftnode,T &rightnode) )
	{
		Node	*node;
		ln.Remove();
		for(node = m_Head;node->m_Next;node = node->m_Next)
		{
			if (gefunc( *node->m_Data, *ln.m_Data )) 
				break;
		}
		ln.m_Next = node;
		ln.m_Prev = node->m_Prev;
		ln.m_Prev->m_Next = node->m_Prev = &ln;
	}

    //-----------------------------------------------------
    //  [use] InsertSorted
	//		Inserts a node into the list in a sorted mannor.  Assumes the list is already sorted and
	//		that the operator >= for <class T> has been defined.
    //
    //  [in/out]  ln
	//		Node to be inserted.
    //
    void    InsertSorted    ( Node &ln )
	{
		Node	*node;
		ln.Remove();
		for(node = m_Head;node->m_Next;node = node->m_Next)
		{
			if (*node->m_Data >= *ln.m_Data )
				break;
		}
		ln.m_Next = node;
		ln.m_Prev = node->m_Prev;
		ln.m_Prev->m_Next = node->m_Prev = &ln;
	}



    //-----------------------------------------------------
    //  [use] Find
    //			Find the node which has data pointer pdata.
    //  [in]  pdata
    //			data pointer to find
    //  [out] Node*
    //			Pointer to node that contains data pointer pdata
    //			or NULL if not found
    Node * Find( const T *pdata ) const
	{
		Node *pn = GetFirstNode();
		for(;pn;pn = pn->GetNextNode())
		{
			if (pn->GetData() == pdata)
				return pn;
		}
		return NULL;
	}
};


#endif // __TLIST_H




