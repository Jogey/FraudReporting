class ViewHomeEntryForm {

    //Methods
    constructor(target) {
        this.target = target;
        console.log('GLOBALROOTURL: ' + GLOBALROOTURL);
        console.log('View initialized');
        //this.sleepTest();
    }

    async sleepTest() {
        console.log('Sleep test A');
        await Apfco.sleep(2000);
        console.log('Sleep test B');
        await Apfco.sleep(2000);
        console.log('Sleep test C');
    }
}

//Instantiate on page load if matching HTML is found
let ViewHomeEntryFormList = [];
$(function () {
    if (document.body.classList.contains('cHome') && document.body.classList.contains('aEntryForm')) {
        let elementsByClass = document.getElementsByClassName("viewWrapper");
        for (let i = 0; i < elementsByClass.length; i++) {
            ViewHomeEntryFormList.push(new ViewHomeEntryForm(elementsByClass.item(i)));
        };
    }
});