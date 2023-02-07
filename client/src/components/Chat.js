import React from 'react'
import { useEffect, useState } from 'react'
import { Form, Button } from 'react-bootstrap'
import UserIcon from '../img/icons8-person-30.png'
import SendIcon from '../img/icons8-email-send-30.png'


function Chat(props) {

    //const [porukeRcv, setPorukeRcv] = useState([])//poruke koje smo dobili
    //const [porukeSnd, setPorukeSnd] = useState([])//poruke koje smo poslali
    const [poruke, setPoruke] = useState([])
    const [username, setUsername] = useState("")
    const [tekst, setTekst] = useState("")

    useEffect(() => {

        var div = document.getElementById("messageBox");
        div.scrollTop = div.scrollHeight - div.clientHeight;

        setUsername(sessionStorage.getItem("username"))

        const request = {
            method: 'GET',
            headers: {'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}   
        } 
        // const myInterval = setInterval(() => {
        //     setPoruke([])
        // }, 1000);
          // clear out the interval using the id when unmounting the component

        //   if(poruke.length == 0){
            fetch(`https://localhost:7013/Poruka/GetPorukeIzmedjuDvaKor/${props.kor2ID}`, request).then(response => {
                if(response.ok)
                response.json().then((porukelocal) => {
                    setPoruke(porukelocal)      
                })
            })
        // }
        // return () => clearInterval(myInterval);
    }, [])

    const onSubmit = (event) => {
        event.preventDefault()

        if(!username)
        {
            alert('Izostavili ste username')
            return
        }

        const request = {
            method: 'POST',
            headers: {'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
        } 

        fetch(`https://localhost:7013/Poruka/CreatePoruka/${props.username2}/${tekst}`, request).then(response => {
            if(response.ok)
            {
                alert("Msg sent")
            }
            return response.json();
        }).then(data => {
            setPoruke([...poruke, {korisnikSnd: {username: data.userSent}, id: "0", tekst: data.txt, vreme: new Date()}])
        })
      
      
    }

   

    return (
        <div>
            <div className='messageBox' id='messageBox'>
                {poruke.map((poruka) => (poruka.korisnikSnd.username == username) ? 
                    (<div className='myMessageWith' key={poruka.id}>
                        <img className="PanelIcon" src={UserIcon}/> YOU
                        <div className='myMessage'>{poruka.tekst}</div>
                        {new Date(poruka.vreme).toUTCString()}</div>) : 
                    (<div className='theirMessageWith' key={poruka.id}>
                        <img className="PanelIcon" src={UserIcon}/> {poruka.korisnikSnd.username}
                        <div className='theirMessage'>{poruka.tekst}</div>
                        {new Date(poruka.vreme).toUTCString()}</div>)) 
                }
            </div>
            <Form className='sendMessage' onSubmit={onSubmit}>
            <Form.Group className='grMess1'>
                    <Form.Control type='text' placeholder='Type here...' value = {tekst} onChange= { (e) => 
                                setTekst(e.target.value) } />
            </Form.Group>


            <Form.Group className='grMess2'>
                <Button variant="dark" className='messButton' type='submit'>
                  <img className="PanelIcon" src={SendIcon}/>
                </Button>
            </Form.Group>

            </Form>

        </div>
    )
}

export default Chat