import React from 'react'
import { useEffect, useState } from 'react'
import { Container, Row, Col } from 'react-bootstrap'
import CreateMsg from './CreateMsg'
import Chat from './Chat'


function MessageBox() {

    const [showState, setShowState] = useState(0)
    const [kor2ID, setKor2ID] = useState(0)
    const [username2, setUsername2] = useState("")
    const [chats, setChats] = useState([])
    useEffect(() => {

        setShowState(0)

        const request = {
            method: 'GET',
            headers: {'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
        } 
      
        fetch(`https://localhost:7013/Poruka/GetChats`, request).then(response => {
            if(response.ok)
                response.json().then((Chats) => {
                    setChats(Chats);
                    console.log(chats);
                })
        })

    }, [])


    return (
            <div className='messPage'>
            <div className='messSidebar'>
                <h4>CHAT LOG</h4>
                {chats.map((chat) => 
                    <div className='messComponent' key={chat.id} onClick={() =>{setShowState(2); setKor2ID(chat.id); setUsername2(chat.username) }}> 
                         {chat.username }
                    </div>)}
                <div className='messComponent' onClick={() => setShowState(1)}>New chat</div>
            </div>
            <div className='messRight'>
            { (showState == 1) && <CreateMsg setShowState = {setShowState} setKor2ID = {setKor2ID} setUsername2 = {setUsername2}/>}
            { (showState == 2) && <Chat kor2ID = {kor2ID} username2 = {username2} />}
            </div>
            </div>
    )

}

export default MessageBox