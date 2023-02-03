import React from 'react'
import { useEffect, useState } from 'react'
import { Button } from 'react-bootstrap'

function Task(props) {

    const [taskID, setTaskID] = useState(props.taskID)
    const [ime, setIme] = useState(props.ime)
    const [opis, setOpis] = useState(props.opis)
    const [status, setStatus] = useState(props.status)
    const [korisnik, setKorisnik] = useState(props.korisnik)
    const [isFinished, setIsFinished] = useState(false)

    useEffect(() => {
        //console.log(props.taskID)
        if(props.sprintStatus == 0)
            setIsFinished(true)
    }, [])

    const request = { 
        method: 'PUT',
        headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
    }
    //ove tri metode mogu da budu jedna
    const TakeTask = () => {

        fetch('https://localhost:7013/Task/TakeTask/' + taskID, request).then( response => {
            if(response.ok)
            {
                alert("Task has been taken")
                setStatus(2)
                props.test()
            }
        })
    }

    const SendForReview = () => {

        fetch('https://localhost:7013/Task/SendForReview/' + taskID, request).then( response => {
            if(response.ok)
            {
                alert("Task sent for review")
                setStatus(3)
                props.test()
            }
        })
    }

    const Approve = () => {
        fetch('https://localhost:7013/Task/ApproveTask/' + taskID, request).then( response => {
            if(response.ok)
            {
                alert("Task has been approved")
                setStatus(4)
                props.test()
            }
        })
    }

    return (
        <div className='Task'>
            <h1>{ime}</h1>
            <h2>{opis}</h2>
            {korisnik != null && 
                <p>Korisnik: {korisnik}</p>
            }
            {status == 1 && isFinished && <Button variant="dark" className='button' onClick={TakeTask}>Take Task</Button>}
            {status == 2 && isFinished && <Button variant="dark" className='button' onClick={SendForReview}>Send For Review</Button>}
            {status == 3 && props.isLeader && isFinished && <Button variant="dark" className='button' onClick={Approve}>Approve</Button>}
        </div>
    )
}

export default Task