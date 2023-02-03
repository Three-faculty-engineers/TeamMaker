import React from 'react'
import { Card, Button, Container } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'
import { useState, useEffect } from 'react'

function ReqJoinBtn({teamid}) {
    
    const [token, setToken] = useState('')
    
    useEffect(() => {
        setToken(sessionStorage.getItem("jwt"))
    }, [])

    const onClick = async () => {
        const request = {
            method: 'POST',
            headers: {'Authorization' : `bearer ${token}`}
        }
        let poruka = prompt("Unesite poruku", "...")
        if(poruka == null)
            return
        await fetch(`https://localhost:7013/Invite/RequestToJoin/${teamid}/${poruka}`, request).then(r => {
            if(r.ok)
                alert('zahtev je poslat')
        })
    }

    return (
        <Button variant="dark" className='button' onClick={onClick}>Join</Button>
    )
}

export default ReqJoinBtn